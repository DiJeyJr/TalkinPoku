using System;
using System.Collections;
using UnityEngine;

// Una basura del escenario. No detecta gestos: escucha a TouchTarget (tap y hold) y a Draggable
// (arrastre), y segun su tipo decide cual de esos gestos la junta.
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(TouchTarget))]
[RequireComponent(typeof(Draggable))]
public class Basura : MonoBehaviour
{
    [Header("Umbrales")]
    // 48 dp son 0,3 pulgadas, unos 7,6 mm: el blanco tactil minimo de Android. Una lata dibujada
    // chica igual se puede tocar sin apuntar.
    [SerializeField] private float tamanoTactilMm = 7.6f;

    [Header("Caida")]
    [SerializeField] private float distanciaCaida = 2f;
    [SerializeField] private float velocidadCaida = 6f;

    [Header("Pegada")]
    // Mas oscura que las sueltas, para que se note antes de tocarla que no se arrastra.
    [SerializeField] private Color colorPegada = new Color(0.7f, 0.6f, 0.5f);
    [SerializeField] private float segundosTemblor = 0.3f;
    [SerializeField] private float amplitudTemblor = 0.08f;

    public event Action<Basura> Juntada;
    // Se toco una pegada como si fuera suelta: el juego explica que hay que mantenerla apretada.
    public event Action<Basura> PidioAyuda;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D caja;
    private TouchTarget touchTarget;
    private Draggable draggable;
    private Collider2D cesto;
    private TipoBasura tipo;
    private Coroutine temblor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        caja = GetComponent<BoxCollider2D>();
        touchTarget = GetComponent<TouchTarget>();
        draggable = GetComponent<Draggable>();
    }

    private void OnEnable()
    {
        touchTarget.Tocado += AlTocar;
        touchTarget.HoldEmpezado += AlSostener;
        draggable.Soltado += AlSoltar;
    }

    private void OnDisable()
    {
        touchTarget.Tocado -= AlTocar;
        touchTarget.HoldEmpezado -= AlSostener;
        draggable.Soltado -= AlSoltar;
    }

    public void Configurar(Sprite sprite, TipoBasura tipoInicial, Collider2D cestoDeLaEscena, Camera camara)
    {
        spriteRenderer.sprite = sprite;
        tipo = tipoInicial;
        cesto = cestoDeLaEscena;
        // Si no se mete en el cesto queda donde se solto, como basura de verdad.
        draggable.VolverAlOrigen = false;
        // Solo la suelta se arrastra: la pegada no se despega y la del arbol todavia no cayo.
        draggable.enabled = tipo == TipoBasura.Suelta;
        spriteRenderer.color = tipo == TipoBasura.Pegada ? colorPegada : Color.white;
        AjustarCollider(camara);
    }

    private void AjustarCollider(Camera camara)
    {
        // Pixeles de pantalla a unidades de mundo con camara ortografica: la altura visible del mundo
        // es dos veces el orthographicSize.
        float mundoPorPixel = 2f * camara.orthographicSize / Screen.height;
        float minimo = Pantalla.MmAPixeles(tamanoTactilMm) * mundoPorPixel / transform.lossyScale.x;

        Bounds dibujo = spriteRenderer.sprite.bounds;
        caja.offset = dibujo.center;
        caja.size = new Vector2(Mathf.Max(dibujo.size.x, minimo), Mathf.Max(dibujo.size.y, minimo));
    }

    // Lo llama LimpiezaGame cuando se sacude el celular.
    public void Sacudir()
    {
        if (tipo == TipoBasura.EnArbol) StartCoroutine(Caer());
    }

    private void AlTocar()
    {
        if (tipo == TipoBasura.Suelta) Juntar();
        // En el Editor no hay acelerometro: tocar la hoja tambien la hace caer.
        else if (tipo == TipoBasura.EnArbol) StartCoroutine(Caer());
        else if (tipo == TipoBasura.Pegada)
        {
            // Un toque mientras tiembla no arranca otro: el nuevo tomaria como origen la posicion
            // corrida y la basura quedaria fuera de lugar.
            if (temblor == null) temblor = StartCoroutine(Temblar());
            PidioAyuda?.Invoke(this);
        }
    }

    private IEnumerator Temblar()
    {
        Vector3 origen = transform.position;
        for (float t = 0f; t < segundosTemblor; t += Time.deltaTime)
        {
            float lado = Mathf.Sin(t * 60f) * amplitudTemblor;
            transform.position = origen + Vector3.right * lado;
            yield return null;
        }
        transform.position = origen;
        temblor = null;
    }

    private void AlSostener()
    {
        if (tipo == TipoBasura.Pegada) Juntar();
    }

    private void AlSoltar(Vector2 dedo)
    {
        // Se mira el dedo y no el centro de la basura: es lo que el jugador siente que solto.
        if (cesto != null && cesto.OverlapPoint(dedo)) Juntar();
    }

    private IEnumerator Caer()
    {
        // Se cambia el tipo antes de caer para que una segunda sacudida no la tire de nuevo.
        tipo = TipoBasura.Suelta;
        Vector3 destino = transform.position + Vector3.down * distanciaCaida;

        while (transform.position != destino)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadCaida * Time.deltaTime);
            yield return null;
        }

        draggable.enabled = true;
    }

    private void Juntar()
    {
        gameObject.SetActive(false);
        Juntada?.Invoke(this);
    }
}
