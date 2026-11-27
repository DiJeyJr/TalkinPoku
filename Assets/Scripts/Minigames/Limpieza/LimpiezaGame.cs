using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// ============================================================================
// LimpiezaGame.cs  (minijuego "Limpieza")
// QUE HACE: las reglas de la limpieza: arma el escenario (playa, parque,
//           bosque), cuenta la basura juntada y va aclarando el fondo. Se gana
//           al juntar todo. Si hay tiempo limite y se acaba, se cobra la parte.
//           Limpiar un escenario desbloquea el siguiente.
// DONDE ESTA: en el objeto "Minijuego" de la escena Limpieza.
// PARA CAMBIAR (Inspector de "Minijuego"):
//   - escenarios y su orden -> lista "Escenarios" (assets Data/Escenarios/*)
//   - tiempo limite -> segundosLimite (0 = sin limite)
//   - cuanto paga un escenario repetido -> pagoAlRepetir (0.3 = 30%)
//   - monedas y basuras de cada escenario -> el asset de ese escenario
// ============================================================================

// Reglas del minijuego de limpieza: arma el escenario, cuenta la basura juntada y aclara el fondo
// a medida que se limpia. Se gana al juntar todo; si hay tiempo limite y se acaba, se cobra la parte.
public class LimpiezaGame : Minijuego
{
    [Header("Escenarios")]
    // [AJUSTE] Escenarios en orden de desbloqueo.
    // En orden de desbloqueo: playa, parque, bosque.
    [SerializeField] private Escenario[] escenarios;

    [Header("Referencias")]
    [SerializeField] private Camera camara;
    [SerializeField] private SpriteRenderer fondoSucio;
    // Va encima del sucio (Order in Layer mayor) y empieza transparente.
    [SerializeField] private SpriteRenderer fondoLimpio;
    [SerializeField] private Basura prefabBasura;
    [SerializeField] private Transform contenedorBasuras;
    [SerializeField] private Collider2D cesto;
    [SerializeField] private ShakeDetector shakeDetector;
    [SerializeField] private TMP_Text textoEscenario;
    [SerializeField] private TMP_Text textoTiempo;
    // Opcional: si esta, explica como se junta la basura pegada cuando el jugador la toca.
    [SerializeField] private TMP_Text textoAyuda;
    [SerializeField] private float segundosAyuda = 2f;

    [Header("Umbrales")]
    // [AJUSTE] Tiempo limite en segundos.
    // 0 = sin tiempo limite. El GDD lo plantea educativo: por defecto no apura.
    [SerializeField] private float segundosLimite = 0f;
    // Pausa entre la ultima basura y el resultado, para que se vea el lugar limpio.
    [SerializeField] private float segundosAntesDelResultado = 1f;
    // [AJUSTE] Fraccion que paga un escenario ya limpiado antes.
    // Un escenario ya limpio paga menos: si no, repetir el mas facil rinde mas que avanzar.
    [SerializeField, Range(0f, 1f)] private float pagoAlRepetir = 0.3f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoJuntar;
    [SerializeField] private AudioClip sonidoSacudir;

    private readonly List<Basura> basuras = new List<Basura>();
    private Escenario actual;
    // true si este escenario ya se habia limpiado antes (paga menos).
    private bool repetido;
    private int juntadas;
    private float tiempoRestante;
    private int segundosMostrados;
    private bool terminado;

    private void Awake()
    {
        if (camara == null) camara = Camera.main;
    }

    // Sacudir el celular hace caer la basura de los arboles.
    private void OnEnable() => shakeDetector.Sacudido += AlSacudir;

    private void OnDisable() => shakeDetector.Sacudido -= AlSacudir;

    // Arranca: elige escenario, pone los fondos y crea cada basura en su lugar.
    public override void StartGame()
    {
        actual = ElegirEscenario();
        // Se mira antes de jugar: al terminar ya figura como limpio aunque sea la primera vez.
        repetido = RegistroIds.EscenariosLimpios.Contiene(actual.Id);
        juntadas = 0;
        terminado = false;

        fondoSucio.sprite = actual.FondoSucio;
        fondoLimpio.sprite = actual.FondoLimpio;
        textoEscenario.SetText(actual.Nombre);
        ActualizarFondo();

        foreach (BasuraEnEscenario datos in actual.Basuras)
        {
            // La posicion se pasa al instanciar y no despues: Draggable guarda la suya en Awake.
            Basura basura = Instantiate(prefabBasura, datos.posicion, Quaternion.identity, contenedorBasuras);
            basura.Configurar(datos.sprite, datos.tipo, cesto, camara);
            basura.Juntada += AlJuntar;
            basura.PidioAyuda += AlPedirAyuda;
            basuras.Add(basura);
        }
        if (textoAyuda != null) textoAyuda.gameObject.SetActive(false);

        tiempoRestante = segundosLimite;
        segundosMostrados = -1;
        textoTiempo.gameObject.SetActive(segundosLimite > 0f);
    }

    // Devuelve el primer escenario sin limpiar (o uno al azar si estan todos limpios).
    private Escenario ElegirEscenario()
    {
        // El primero que falta limpiar: limpiar uno es lo que desbloquea el siguiente.
        foreach (Escenario escenario in escenarios)
        {
            if (!RegistroIds.EscenariosLimpios.Contiene(escenario.Id)) return escenario;
        }

        // Con todos limpios se repite uno al azar, para que el minijuego siga sirviendo.
        return escenarios[UnityEngine.Random.Range(0, escenarios.Length)];
    }

    // Si hay tiempo limite, lo descuenta y termina al llegar a 0.
    private void Update()
    {
        if (terminado || segundosLimite <= 0f) return;

        tiempoRestante -= Time.deltaTime;
        int segundos = Mathf.Max(0, Mathf.CeilToInt(tiempoRestante));
        // Solo se reescribe el texto cuando cambia el numero.
        if (segundos != segundosMostrados)
        {
            segundosMostrados = segundos;
            textoTiempo.SetText("{0}", segundos);
        }

        if (tiempoRestante <= 0f) Terminar();
    }

    private void AlSacudir()
    {
        if (terminado) return;

        SoundPlayer.Reproducir(sonidoSacudir);
        foreach (Basura basura in basuras) basura.Sacudir();
    }

    // Tocaron una basura pegada: muestra el texto de ayuda un rato.
    private void AlPedirAyuda(Basura basura)
    {
        if (textoAyuda == null) return;
        StopCoroutine(nameof(MostrarAyuda));
        StartCoroutine(nameof(MostrarAyuda));
    }

    private IEnumerator MostrarAyuda()
    {
        textoAyuda.gameObject.SetActive(true);
        yield return new WaitForSeconds(segundosAyuda);
        textoAyuda.gameObject.SetActive(false);
    }

    // Se junto una basura: suma, aclara el fondo y, si era la ultima, termina.
    private void AlJuntar(Basura basura)
    {
        juntadas++;
        SoundPlayer.Reproducir(sonidoJuntar);
        ActualizarFondo();

        if (juntadas == basuras.Count) Terminar();
    }

    // La opacidad del fondo limpio = basura juntada / total.
    private void ActualizarFondo()
    {
        // El lugar se recupera de a poco: el fondo limpio va apareciendo encima del sucio.
        float progreso = basuras.Count == 0 ? 0f : juntadas / (float)basuras.Count;
        Color color = fondoLimpio.color;
        color.a = progreso;
        fondoLimpio.color = color;
    }

    // Fin: registra el escenario si se limpio todo y calcula las monedas.
    private void Terminar()
    {
        terminado = true;
        bool limpioTodo = juntadas == basuras.Count;
        // Lo que quedo sin juntar no se puede seguir tocando debajo de la pantalla de resultado.
        contenedorBasuras.gameObject.SetActive(false);

        if (limpioTodo)
        {
            RegistroIds.EscenariosLimpios.Registrar(actual.Id);
            // Se guarda ya y no en el proximo autoguardado: desbloquear un escenario no se tiene que perder.
            SaveSystem.Guardar();
        }

        // Si se acabo el tiempo se paga la parte limpiada: el feedback celebra lo hecho, no castiga.
        float pago = actual.Monedas * juntadas / (float)Mathf.Max(1, basuras.Count);
        if (repetido) pago *= pagoAlRepetir;
        int monedas = Mathf.RoundToInt(pago);
        Terminar(new MinigameResult(juntadas, monedas, true, limpioTodo), segundosAntesDelResultado);
    }
}
