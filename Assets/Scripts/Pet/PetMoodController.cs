using UnityEngine;

/// Elige la cara de Poku. Contento es una reaccion de unos segundos (al volver de un minijuego o
/// cuando la pelota le pega). El resto del tiempo manda la necesidad que esta peor si ya se nota
/// (debajo del umbral bajo de NeedsConfig), y si ninguna se nota, la cara normal.
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

    [Header("Expresiones")]
    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite contento;
    [SerializeField] private Sprite conHambre;
    [SerializeField] private Sprite sucio;
    [SerializeField] private Sprite aburrido;
    [SerializeField] private Sprite cansado;

    private SpriteRenderer spriteRenderer;
    private bool reaccionando;
    private float contentoHasta;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        necesidades.NeedChanged += AlCambiar;
    }

    private void OnDisable()
    {
        necesidades.NeedChanged -= AlCambiar;
    }

    private void Start()
    {
        Actualizar();
    }

    private void Update()
    {
        // Ningun evento avisa cuando pasan los segundos de la reaccion: se mira el reloj.
        if (reaccionando && Time.time >= contentoHasta)
        {
            reaccionando = false;
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

    private void AlCambiar(NeedType tipo, float valor)
    {
        Actualizar();
    }

    private void Actualizar()
    {
        Sprite cara = Elegir();
        if (spriteRenderer.sprite != cara) spriteRenderer.sprite = cara;
    }

    private Sprite Elegir()
    {
        // La reaccion tapa a las demas caras: es corta y es la respuesta a algo que hizo el jugador.
        if (reaccionando) return contento;

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
                case NeedType.Hambre: return conHambre;
                case NeedType.Higiene: return sucio;
                case NeedType.Diversion: return aburrido;
                default: return cansado;
            }
        }

        return normal;
    }
}
