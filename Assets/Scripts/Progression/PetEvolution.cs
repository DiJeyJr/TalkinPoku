using UnityEngine;

/// Va en la mascota. Al arrancar le pone la apariencia de la etapa en la que esta, y cuando la
/// progresion la hace evolucionar la cambia con un "pop", el cartel y un sonido. Lo mismo al
/// activarse y terminarse la Mega Evolucion.
[RequireComponent(typeof(PetMoodController))]
[RequireComponent(typeof(SpriteRenderer))]
public class PetEvolution : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Progression progreso;
    // Opcional: sin ella nunca se ve Mega-Pokuchan.
    [SerializeField] private MegaEvolution mega;
    [SerializeField] private LevelUpBanner cartel;

    [Header("Feedback")]
    [SerializeField] private float escalaPop = 1.35f;
    [SerializeField] private float duracionPop = 0.6f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoEvolucion;

    private PetMoodController cara;
    private SpriteRenderer spriteRenderer;
    private Vector3 escalaBase;
    private float escalaEtapa = 1f;
    private float extra;
    private int etapaActual;

    private void Awake()
    {
        cara = GetComponent<PetMoodController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        escalaBase = transform.localScale;
    }

    private void OnEnable()
    {
        progreso.EtapaCambiada += Evolucionar;
        if (mega != null) mega.MegaCambiada += AlCambiarMega;
    }

    private void OnDisable()
    {
        progreso.EtapaCambiada -= Evolucionar;
        if (mega != null) mega.MegaCambiada -= AlCambiarMega;
    }

    private void Start()
    {
        // Sin festejo: al abrir la app ya estaba en esa etapa.
        etapaActual = progreso.Etapa;
        Aplicar(AparienciaActual());
    }

    private void Update()
    {
        if (extra <= 0f) return;
        // Igual que Latido: crece de golpe y vuelve a su tamano.
        extra = Mathf.MoveTowards(extra, 0f, (escalaPop - 1f) / duracionPop * Time.deltaTime);
        transform.localScale = escalaBase * escalaEtapa * (1f + extra);
    }

    private void Evolucionar(int etapa)
    {
        // Reiniciar desde el panel de admin tambien cambia de etapa, pero para atras: eso no se festeja.
        bool subio = etapa > etapaActual;
        string antes = progreso.Config.Etapas[etapaActual].nombre;
        etapaActual = etapa;
        Aplicar(AparienciaActual());
        if (!subio) return;

        Festejar("¡" + antes + " evolucionó a " + progreso.Config.Etapas[etapa].nombre + "!");
    }

    private void AlCambiarMega(bool activa)
    {
        string nombre = progreso.Config.Etapas[etapaActual].nombre;
        Aplicar(AparienciaActual());
        // Al terminarse vuelve sin pop ni sonido: no es un logro, solo avisa por que cambio.
        if (activa) Festejar("¡" + nombre + " Mega Evolucionó!");
        else if (cartel != null) cartel.Mostrar("Mega-" + nombre + " volvió a ser " + nombre);
    }

    private PetAppearance AparienciaActual()
    {
        PetAppearance megaApariencia = progreso.Config.AparienciaMega;
        if (mega != null && MegaEvolution.Activa && megaApariencia != null) return megaApariencia;
        return progreso.Config.Etapas[etapaActual].apariencia;
    }

    private void Festejar(string mensaje)
    {
        extra = escalaPop - 1f;
        SoundPlayer.Reproducir(sonidoEvolucion);
        if (cartel != null) cartel.Mostrar(mensaje);
    }

    private void Aplicar(PetAppearance apariencia)
    {
        if (apariencia == null) return;

        cara.CambiarApariencia(apariencia);
        spriteRenderer.color = apariencia.Tinte;
        escalaEtapa = apariencia.Escala;
        transform.localScale = escalaBase * escalaEtapa;
    }
}
