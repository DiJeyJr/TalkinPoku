using UnityEngine;

// Va en la mascota. Al arrancar le pone la apariencia de la etapa en la que esta, y cuando la
// progresion la hace evolucionar la cambia con un "pop", el cartel y un sonido. Lo mismo al
// activarse y terminarse la Mega Evolucion. Tambien festeja la subida de nivel con un salto.
// Son los eventos mas raros del juego: llevan todo (pausa, vibracion, particulas) y cada uno un
// poco mas que el anterior, para que la Mega se sienta la mas grande.
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
    [SerializeField] private float escalaPopMega = 1.5f;
    [SerializeField] private float duracionPop = 0.6f;
    // Todo queda quieto un instante en el pico del pop, asi el ojo registra el cambio. Mas de 0.3 s
    // ya se lee como que el juego se trabo.
    [SerializeField] private float pausaNivel = 0.08f;
    [SerializeField] private float pausaEvolucion = 0.15f;
    [SerializeField] private float pausaMega = 0.25f;

    [Header("Particulas")]
    // Opcional: un ParticleSystem hijo de la mascota, con Play On Awake apagado.
    [SerializeField] private ParticleSystem particulas;
    [SerializeField] private int particulasNivel = 12;
    [SerializeField] private int particulasEvolucion = 25;
    [SerializeField] private int particulasMega = 40;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoEvolucion;

    private PetMoodController cara;
    private PetJuice juice;
    private Vector3 escalaBase;
    private float escalaEtapa = 1f;
    private float extra;
    private int etapaActual;
    private bool pausado;
    private float reanudarEn;
    private float escalaDeTiempo = 1f;

    private void Awake()
    {
        cara = GetComponent<PetMoodController>();
        juice = GetComponent<PetJuice>();
        escalaBase = transform.localScale;
    }

    private void OnEnable()
    {
        progreso.EtapaCambiada += Evolucionar;
        progreso.NivelSubido += AlSubirNivel;
        if (mega != null) mega.MegaCambiada += AlCambiarMega;
    }

    private void OnDisable()
    {
        progreso.EtapaCambiada -= Evolucionar;
        progreso.NivelSubido -= AlSubirNivel;
        if (mega != null) mega.MegaCambiada -= AlCambiarMega;
        // Si se cambia de escena en plena pausa, el juego no puede quedar congelado.
        if (pausado) Reanudar();
    }

    private void Start()
    {
        // Sin festejo: al abrir la app ya estaba en esa etapa.
        etapaActual = progreso.Etapa;
        Aplicar(progreso.Config.AparienciaPara(progreso.Nivel));
    }

    private void Update()
    {
        // Tiempo sin escala: con timeScale en 0 Time.time no avanza y la pausa no terminaria nunca.
        if (pausado && Time.unscaledTime >= reanudarEn) Reanudar();

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
        Aplicar(progreso.Config.AparienciaPara(progreso.Nivel));
        if (!subio) return;

        Festejar("¡" + antes + " evolucionó a " + progreso.Config.Etapas[etapa].nombre + "!",
            escalaPop, pausaEvolucion, particulasEvolucion);
    }

    private void AlCambiarMega(bool activa)
    {
        string nombre = progreso.Config.Etapas[etapaActual].nombre;
        Aplicar(progreso.Config.AparienciaPara(progreso.Nivel));
        // Al terminarse vuelve sin pop ni sonido: no es un logro, solo avisa por que cambio.
        if (activa) Festejar("¡" + nombre + " Mega Evolucionó!", escalaPopMega, pausaMega, particulasMega);
        else if (cartel != null) cartel.Mostrar("Mega-" + nombre + " volvió a ser " + nombre);
    }

    // El cartel y el sonido del nivel los pone LevelUpBanner; aca va lo que le pasa a la mascota.
    private void AlSubirNivel(int nivel)
    {
        if (juice != null) juice.Saltar();
        Impacto(pausaNivel, particulasNivel);
    }

    private void Festejar(string mensaje, float escala, float pausa, int cantidadParticulas)
    {
        extra = escala - 1f;
        SoundPlayer.Reproducir(sonidoEvolucion);
        if (cartel != null) cartel.Mostrar(mensaje);
        Impacto(pausa, cantidadParticulas);
    }

    private void Impacto(float pausa, int cantidadParticulas)
    {
        Vibracion.Vibrar();
        if (particulas != null) particulas.Emit(cantidadParticulas);
        Pausar(pausa);
    }

    private void Pausar(float segundos)
    {
        if (segundos <= 0f) return;
        // Si ya estaba en pausa (subir de nivel y evolucionar llegan juntos) se estira, sin pisar la
        // escala que habia antes.
        if (!pausado) escalaDeTiempo = Time.timeScale;
        pausado = true;
        Time.timeScale = 0f;
        reanudarEn = Mathf.Max(reanudarEn, Time.unscaledTime + segundos);
    }

    private void Reanudar()
    {
        pausado = false;
        Time.timeScale = escalaDeTiempo;
    }

    private void Aplicar(PetAppearance apariencia)
    {
        if (apariencia == null) return;

        cara.CambiarApariencia(apariencia);
        escalaEtapa = apariencia.Escala;
        transform.localScale = escalaBase * escalaEtapa;
    }
}
