using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// Reglas del minijuego de limpieza: arma el escenario, cuenta la basura juntada y aclara el fondo
/// a medida que se limpia. Se gana al juntar todo; si hay tiempo limite y se acaba, se cobra la parte.
public class LimpiezaGame : MonoBehaviour, IMinigame
{
    [Header("Escenarios")]
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
    // 0 = sin tiempo limite. El GDD lo plantea educativo: por defecto no apura.
    [SerializeField] private float segundosLimite = 0f;
    // Pausa entre la ultima basura y el resultado, para que se vea el lugar limpio.
    [SerializeField] private float segundosAntesDelResultado = 1f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoJuntar;
    [SerializeField] private AudioClip sonidoSacudir;

    public event Action<MinigameResult> Finished;

    private readonly List<Basura> basuras = new List<Basura>();
    private Escenario actual;
    private int juntadas;
    private float tiempoRestante;
    private int segundosMostrados;
    private bool terminado;

    private void Awake()
    {
        if (camara == null) camara = Camera.main;
    }

    private void OnEnable() => shakeDetector.Sacudido += AlSacudir;

    private void OnDisable() => shakeDetector.Sacudido -= AlSacudir;

    public void StartGame()
    {
        actual = ElegirEscenario();
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

    private Escenario ElegirEscenario()
    {
        // El primero que falta limpiar: limpiar uno es lo que desbloquea el siguiente.
        foreach (Escenario escenario in escenarios)
        {
            if (!EscenariosLimpios.EstaLimpio(escenario.Id)) return escenario;
        }

        // Con todos limpios se repite uno al azar, para que el minijuego siga sirviendo.
        return escenarios[UnityEngine.Random.Range(0, escenarios.Length)];
    }

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

    private void AlJuntar(Basura basura)
    {
        juntadas++;
        SoundPlayer.Reproducir(sonidoJuntar);
        ActualizarFondo();

        if (juntadas == basuras.Count) Terminar();
    }

    private void ActualizarFondo()
    {
        // El lugar se recupera de a poco: el fondo limpio va apareciendo encima del sucio.
        float progreso = basuras.Count == 0 ? 0f : juntadas / (float)basuras.Count;
        Color color = fondoLimpio.color;
        color.a = progreso;
        fondoLimpio.color = color;
    }

    private void Terminar()
    {
        terminado = true;
        bool limpioTodo = juntadas == basuras.Count;
        // Lo que quedo sin juntar no se puede seguir tocando debajo de la pantalla de resultado.
        contenedorBasuras.gameObject.SetActive(false);

        if (limpioTodo)
        {
            EscenariosLimpios.Registrar(actual.Id);
            // Se guarda ya y no en el proximo autoguardado: desbloquear un escenario no se tiene que perder.
            SaveSystem.Guardar();
        }

        StartCoroutine(TerminarConDemora(limpioTodo));
    }

    private IEnumerator TerminarConDemora(bool limpioTodo)
    {
        yield return new WaitForSeconds(segundosAntesDelResultado);

        // Si se acabo el tiempo se paga la parte limpiada: el feedback celebra lo hecho, no castiga.
        int monedas = Mathf.RoundToInt(actual.Monedas * juntadas / (float)basuras.Count);
        Finished?.Invoke(new MinigameResult(juntadas, monedas, true, limpioTodo));
    }
}
