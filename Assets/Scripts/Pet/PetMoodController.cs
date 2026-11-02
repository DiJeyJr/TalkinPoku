using UnityEngine;

// Elige la cara de Poku. Dormido y acariciado duran lo que dura eso. Contento es una reaccion de
// unos segundos (al volver de un minijuego o cuando la pelota le pega). El resto del tiempo manda
// la necesidad que esta peor si ya se nota (debajo del umbral bajo de NeedsConfig), y si ninguna
// se nota, la cara normal. Las caras salen de la apariencia de la etapa actual (Poku, Pokucha...).
[RequireComponent(typeof(SpriteRenderer))]
public class PetMoodController : MonoBehaviour
{
    // Felicidad queda afuera: es el promedio de estas cuatro, nunca puede ser la peor.
    private static readonly NeedType[] necesidadesBase =
    {
        NeedType.Hambre, NeedType.Higiene, NeedType.Diversion, NeedType.Energia
    };

    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    // Opcional: el mismo TouchTarget que usa PetInteraction, para poner la cara de caricia.
    [SerializeField] private TouchTarget toque;
    // Opcional: sin el, las emociones cambian la cara pero el cuerpo no se mueve.
    [SerializeField] private PetJuice juice;

    [Header("Expresiones")]
    // La de Poku. PetEvolution la cambia al arrancar si la mascota ya evoluciono.
    [SerializeField] private PetAppearance apariencia;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoFeliz;
    [SerializeField] private AudioClip sonidoTriste;
    [SerializeField] private AudioClip sonidoEnojado;
    [SerializeField] private float volumenEmocion = 0.8f;
    // Si una necesidad oscila justo en el umbral, la emocion cambiaria seguido y sonaria en loop.
    [SerializeField] private float esperaEntreSonidos = 3f;

    // No hay cara de enojado: Poku se enoja cuando esta sucio.
    private enum Emocion { Ninguna, Feliz, Triste, Enojado }

    private SpriteRenderer spriteRenderer;
    private Emocion emocion;
    private bool arrancado;
    private float silencioHasta;
    private bool reaccionando;
    private float contentoHasta;
    private bool acariciando;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        necesidades.NeedChanged += AlCambiar;
        necesidades.DormidoCambiado += AlDormir;
    }

    private void OnDisable()
    {
        necesidades.NeedChanged -= AlCambiar;
        necesidades.DormidoCambiado -= AlDormir;
    }

    private void Start()
    {
        Actualizar();
        // Recien ahora vale una emocion nueva: la de arranque no suena ni se mueve.
        arrancado = true;
    }

    private void Update()
    {
        // Ningun evento avisa cuando pasan los segundos de la reaccion: se mira el reloj.
        if (reaccionando && Time.time >= contentoHasta)
        {
            reaccionando = false;
            Actualizar();
        }

        // Se mira Sosteniendo y no los eventos de hold: si se abre una pantalla con el dedo apoyado,
        // el TouchTarget se apaga sin avisar HoldTerminado y la cara quedaria trabada.
        bool ahora = toque != null && toque.Sosteniendo;
        if (ahora != acariciando)
        {
            acariciando = ahora;
            Actualizar();
        }
    }

    // La llaman quienes lo divierten (el premio del minijuego, la pelota). Si ya estaba contento,
    // alarga la reaccion en vez de cortarla.
    public void Alegrar(float segundos)
    {
        contentoHasta = Mathf.Max(contentoHasta, Time.time + segundos);
        reaccionando = true;
        Actualizar();
    }

    public PetAppearance Apariencia => apariencia;

    // PetAccessories la escucha para acomodar la ropa a la etapa nueva.
    public event System.Action AparienciaCambiada;

    public void CambiarApariencia(PetAppearance nueva)
    {
        if (nueva == null || nueva == apariencia) return;
        apariencia = nueva;
        Actualizar();
        AparienciaCambiada?.Invoke();
    }

    private void AlCambiar(NeedType tipo, float valor)
    {
        Actualizar();
    }

    private void AlDormir(bool estaDormido)
    {
        Actualizar();
    }

    private void Actualizar()
    {
        Sprite cara = Elegir(out Emocion nueva);
        if (spriteRenderer.sprite != cara) spriteRenderer.sprite = cara;

        // Solo al cambiar: Actualizar corre con cada cambio de necesidad y sonaria todo el tiempo.
        if (nueva == emocion) return;
        emocion = nueva;
        if (arrancado) Expresar(nueva);
    }

    private void Expresar(Emocion nueva)
    {
        if (nueva == Emocion.Ninguna) return;

        // Si acaricia, PetInteraction ya la aplasta: rebotar encima seria doble.
        if (juice != null)
        {
            if (nueva == Emocion.Feliz && !acariciando) juice.Saltar();
            else if (nueva == Emocion.Enojado) juice.Sacudir();
            else if (nueva == Emocion.Triste) juice.Desinflar();
        }

        if (Time.time < silencioHasta) return;
        silencioHasta = Time.time + esperaEntreSonidos;

        switch (nueva)
        {
            case Emocion.Feliz: SoundPlayer.Reproducir(sonidoFeliz, volumenEmocion); break;
            case Emocion.Triste: SoundPlayer.Reproducir(sonidoTriste, volumenEmocion); break;
            default: SoundPlayer.Reproducir(sonidoEnojado, volumenEmocion); break;
        }
    }

    private Sprite Elegir(out Emocion nueva)
    {
        nueva = Emocion.Ninguna;

        // Dormido tapa todo: ni la pelota ni las caricias lo despiertan (PetInteraction tampoco cuenta
        // las caricias mientras duerme). Sin sprite asignado sigue con las demas caras.
        if (necesidades.Dormido && apariencia.Dormido != null) return apariencia.Dormido;

        // Acariciarlo y la reaccion tapan a las necesidades: son la respuesta a algo que hizo el jugador.
        if (acariciando && apariencia.Acariciado != null)
        {
            nueva = Emocion.Feliz;
            return apariencia.Acariciado;
        }
        if (reaccionando)
        {
            nueva = Emocion.Feliz;
            return apariencia.Contento;
        }

        NeedType peor = necesidadesBase[0];
        foreach (NeedType tipo in necesidadesBase)
        {
            if (necesidades.Obtener(tipo) < necesidades.Obtener(peor)) peor = tipo;
        }

        // Con el umbral bajo y no con el critico: si no, Poku podria estar lleno de manchas y
        // seguir con la cara normal.
        if (necesidades.Obtener(peor) < necesidades.UmbralBajo)
        {
            switch (peor)
            {
                case NeedType.Hambre:
                    nueva = Emocion.Triste;
                    return apariencia.ConHambre;
                case NeedType.Higiene:
                    nueva = Emocion.Enojado;
                    return apariencia.Sucio;
                case NeedType.Diversion:
                    nueva = Emocion.Triste;
                    return apariencia.Aburrido;
                default:
                    nueva = Emocion.Triste;
                    return apariencia.Cansado;
            }
        }

        return apariencia.Normal;
    }
}
