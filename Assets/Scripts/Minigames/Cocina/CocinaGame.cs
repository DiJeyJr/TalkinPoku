using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// Reglas de la cocina: con dos o tres ingredientes en la olla, mezclar (sacudiendo el celular o con
/// el boton Mezclar) busca la receta. Una nueva paga mas que una repetida y una que no existe no
/// cobra nada: es un juego de descubrir, y equivocarse tiene que ser gratis.
public class CocinaGame : MonoBehaviour, IMinigame
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
    [SerializeField] private RecipeDefinition[] recetas;

    [Header("Recompensa")]
    [SerializeField] private int monedasRecetaNueva = 20;
    [SerializeField] private int monedasRecetaRepetida = 5;
    [SerializeField] private int maximoMonedas = 100;

    [Header("Feedback")]
    [SerializeField] private int minimoIngredientes = 2;
    [SerializeField] private float segundosPlato = 1.5f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoRecetaNueva;
    // Una repetida igual paga monedas: suena la moneda.
    [SerializeField] private AudioClip sonidoRecetaRepetida;
    [SerializeField] private AudioClip sonidoPuaj;

    public event Action<MinigameResult> Finished;

    private readonly Dictionary<string, RecipeDefinition> porClave = new Dictionary<string, RecipeDefinition>();
    private InputAction mezclar;
    private int monedas;
    private int cocinadas;
    private float ocultarPlatoEn;
    private bool terminado;

    public void StartGame()
    {
        // Se arma una vez: despues, probar una combinacion es una sola consulta al diccionario.
        porClave.Clear();
        foreach (RecipeDefinition receta in recetas) porClave[receta.Clave] = receta;

        // Como en la clase 4: la accion se busca una vez y el Update solo pregunta si se apreto.
        // El boton Mezclar de la pantalla es un OnScreenButton que aprieta esa accion.
        mezclar = InputSystem.actions.FindAction("Mezclar");
        shake.Sacudido += Cocinar;
        botonListo.onClick.AddListener(Terminar);

        monedas = 0;
        cocinadas = 0;
        terminado = false;
        plato.enabled = false;
        textoMonedas.SetText("{0}", monedas);
        textoMensaje.SetText("Arrastrá ingredientes a la olla");
        libro.Mostrar(recetas);
    }

    private void OnDestroy()
    {
        if (shake != null) shake.Sacudido -= Cocinar;
    }

    private void Update()
    {
        if (terminado) return;

        if (mezclar != null && mezclar.WasPressedThisFrame()) Cocinar();

        if (plato.enabled && Time.time >= ocultarPlatoEn) plato.enabled = false;
    }

    private void Cocinar()
    {
        // Con el libro abierto la olla esta apagada: sacudir ahi no cocina.
        if (terminado || !olla.isActiveAndEnabled) return;

        if (olla.Contenido.Count < minimoIngredientes)
        {
            textoMensaje.SetText("Poné al menos {0} ingredientes", minimoIngredientes);
            return;
        }

        string clave = RecipeDefinition.ClaveDe(olla.Contenido.Select(i => i.Id));
        olla.Vaciar();

        if (!porClave.TryGetValue(clave, out RecipeDefinition receta))
        {
            textoMensaje.SetText("¡Puaj! Eso no es una receta");
            SoundPlayer.Reproducir(sonidoPuaj);
            return;
        }

        bool nueva = RecipeBook.Registrar(receta.Id);
        cocinadas++;

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

    private void Terminar()
    {
        if (terminado) return;
        terminado = true;

        // Entrar y salir sin cocinar nada cuenta como abandonar: no paga ni divierte.
        Finished?.Invoke(new MinigameResult(cocinadas, monedas, cocinadas > 0));
    }
}
