using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// ============================================================================
// CocinaGame.cs  (minijuego "Cocina")
// QUE HACE: las reglas de la cocina. Con 2 o 3 ingredientes en la olla, mezclar
//           (sacudiendo el celular o con el boton Mezclar) busca si esa
//           combinacion es una receta. Receta nueva paga mas que una repetida;
//           una combinacion que no existe no gasta nada. El plato cocinado va a
//           la despensa y aparece en la mesada de la casa.
// DONDE ESTA: en el objeto "Minijuego" de la escena Cocina.
// PARA CAMBIAR (Inspector de "Minijuego"):
//   - recetas disponibles -> lista "Recetas" (assets Data/Cocina/Receta_*)
//   - monedas por receta nueva / repetida / maximo -> monedasRecetaNueva,
//     monedasRecetaRepetida, maximoMonedas
//   - ingredientes minimos -> minimoIngredientes
//   - cuantas mezclas hacen falta -> mezclasIniciales / mezclasMaximas
// ============================================================================

// Reglas de la cocina: con dos o tres ingredientes en la olla, mezclar (sacudiendo el celular o con
// el boton Mezclar) busca la receta. Una nueva paga mas que una repetida y una que no existe no
// cobra nada: es un juego de descubrir, y equivocarse tiene que ser gratis.
public class CocinaGame : Minijuego
{
    [Header("Referencias")]
    [SerializeField] private Olla olla;
    [SerializeField] private ShakeDetector shake;
    [SerializeField] private LibroDeRecetas libro;
    [SerializeField] private SpriteRenderer plato;
    [SerializeField] private TMP_Text textoMensaje;
    [SerializeField] private TMP_Text textoMonedas;
    [SerializeField] private Button botonListo;

    [Header("Recetas")]
    // [AJUSTE] Recetas que existen.
    [SerializeField] private RecipeDefinition[] recetas;

    [Header("Recompensa")]
    // [AJUSTE] Monedas por receta nueva, por repetida y maximo por partida.
    [SerializeField] private int monedasRecetaNueva = 20;
    [SerializeField] private int monedasRecetaRepetida = 5;
    [SerializeField] private int maximoMonedas = 100;

    [Header("Feedback")]
    // [AJUSTE] Ingredientes minimos para poder cocinar.
    [SerializeField] private int minimoIngredientes = 2;
    // Segundos que se ve el plato cocinado.
    [SerializeField] private float segundosPlato = 1.5f;

    [Header("Dificultad")]
    // [AJUSTE] Mezclas necesarias al inicio y como maximo.
    // Cuantas veces hay que mezclar (sacudir o apretar Mezclar) para que la olla cocine. Arranca en
    // las iniciales y con las recetas cocinadas se acerca al tope: no castiga equivocarse, pide mas trabajo.
    [SerializeField] private int mezclasIniciales = 1;
    [SerializeField] private int mezclasMaximas = 4;
    // Recetas cocinadas en la partida para llegar al tope.
    [SerializeField] private int recetasParaMaximo = 8;
    // Igual que en Vuelo. Eje X: avance (0 a 1) hasta el tope; eje Y: cuanto del camino ya se recorrio.
    [SerializeField] private AnimationCurve curvaDificultad = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoRecetaNueva;
    // Una repetida igual paga monedas: suena la moneda.
    [SerializeField] private AudioClip sonidoRecetaRepetida;
    [SerializeField] private AudioClip sonidoPuaj;

    // Clave de ingredientes (ej "harina+huevo") -> receta.
    private readonly Dictionary<string, RecipeDefinition> porClave = new Dictionary<string, RecipeDefinition>();
    private InputAction mezclar;
    private int monedas;
    // Recetas cocinadas en esta partida (tambien es el puntaje).
    private int cocinadas;
    private float ocultarPlatoEn;
    private int mezclasNecesarias;
    private int mezclasHechas;
    private bool terminado;

    // Arranca: arma el diccionario de recetas, se engancha al shake y al boton, y muestra el libro.
    public override void StartGame()
    {
        // Se arma una vez: despues, probar una combinacion es una sola consulta al diccionario.
        porClave.Clear();
        foreach (RecipeDefinition receta in recetas) porClave[receta.Clave] = receta;

        // Como en la clase 4: la accion se busca una vez y el Update solo pregunta si se apreto.
        // El boton Mezclar de la pantalla es un OnScreenButton que aprieta esa accion.
        mezclar = InputSystem.actions.FindAction("Mezclar");
        shake.Sacudido += Cocinar;
        olla.ContenidoCambiado += ReiniciarMezcla;
        botonListo.onClick.AddListener(Terminar);

        monedas = 0;
        cocinadas = 0;
        ActualizarDificultad();
        terminado = false;
        plato.enabled = false;
        textoMonedas.SetText("{0}", monedas);
        // Los ingredientes se compran en la tienda: sin ninguno no hay nada para cocinar.
        textoMensaje.SetText(Despensa.Ingredientes.Total > 0
            ? "Arrastrá ingredientes a la olla"
            : "No tenés ingredientes: compralos en la Tienda");
        libro.Mostrar(recetas);
    }

    private void OnDestroy()
    {
        if (shake != null) shake.Sacudido -= Cocinar;
        if (olla != null) olla.ContenidoCambiado -= ReiniciarMezcla;
    }

    // Mira si se apreto el boton Mezclar y esconde el plato cuando pasa el tiempo.
    private void Update()
    {
        if (terminado) return;

        if (mezclar != null && mezclar.WasPressedThisFrame()) Cocinar();

        if (plato.enabled && Time.time >= ocultarPlatoEn) plato.enabled = false;
    }

    // Una mezcla: cuando se juntan las necesarias, busca la receta y cocina (o "puaj").
    private void Cocinar()
    {
        // Con el libro abierto la olla esta apagada: sacudir ahi no cocina.
        if (terminado || !olla.isActiveAndEnabled) return;

        if (olla.Contenido.Count < minimoIngredientes)
        {
            textoMensaje.SetText("Poné al menos {0} ingredientes", minimoIngredientes);
            return;
        }

        // La olla vacia (receta o puaj) dispara ContenidoCambiado y la cuenta vuelve a cero sola.
        mezclasHechas++;
        if (mezclasHechas < mezclasNecesarias)
        {
            textoMensaje.SetText("¡Seguí mezclando! Faltan {0}", mezclasNecesarias - mezclasHechas);
            return;
        }

        string clave = RecipeDefinition.ClaveDe(olla.Contenido.Select(i => i.Id));

        if (!porClave.TryGetValue(clave, out RecipeDefinition receta))
        {
            // Una combinacion que no existe no gasta nada: los ingredientes vuelven al estante.
            // Equivocarse no cuesta, asi el jugador se anima a probar.
            olla.Vaciar();
            textoMensaje.SetText("¡Puaj! Eso no es una receta");
            SoundPlayer.Reproducir(sonidoPuaj);
            return;
        }

        // Cocinar si gasta lo que habia en la olla. El plato va al inventario y aparece en la mesada
        // de la casa para darselo a Poku.
        foreach (IngredientDefinition ingrediente in olla.Contenido) Despensa.Ingredientes.Gastar(ingrediente.Id);
        olla.Vaciar();
        Despensa.Platos.Sumar(receta.Id);

        bool nueva = RegistroIds.Recetas.Registrar(receta.Id);
        cocinadas++;
        ActualizarDificultad();

        // Tope por partida: repetir la misma receta muchas veces no rompe la economia.
        int premio = nueva ? monedasRecetaNueva : monedasRecetaRepetida;
        monedas = Mathf.Min(monedas + premio, maximoMonedas);
        textoMonedas.SetText("{0}", monedas);
        textoMensaje.SetText(nueva ? "¡Receta nueva: " + receta.Nombre + "!" : receta.Nombre + " otra vez");
        SoundPlayer.Reproducir(nueva ? sonidoRecetaNueva : sonidoRecetaRepetida);

        plato.sprite = receta.Sprite;
        plato.enabled = true;
        ocultarPlatoEn = Time.time + segundosPlato;

        if (nueva) libro.Mostrar(recetas);
    }

    // Si cambian los ingredientes de la olla, se vuelve a contar desde cero.
    private void ReiniciarMezcla()
    {
        mezclasHechas = 0;
    }

    // Mezclas necesarias segun cuantas recetas se cocinaron.
    private void ActualizarDificultad()
    {
        float k = Avance(cocinadas, recetasParaMaximo, curvaDificultad);
        mezclasNecesarias = Mathf.RoundToInt(Mathf.Lerp(mezclasIniciales, mezclasMaximas, k));
    }

    // Boton "Listo": termina la partida.
    private void Terminar()
    {
        if (terminado) return;
        terminado = true;

        // Entrar y salir sin cocinar nada cuenta como abandonar: no paga ni divierte, y tampoco es
        // victoria. Con una receta cocinada ya se gana: equivocarse no tiene que castigar.
        bool cocino = cocinadas > 0;
        Terminar(new MinigameResult(cocinadas, monedas, cocino, cocino));
    }
}
