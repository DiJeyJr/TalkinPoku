using UnityEngine;
using UnityEngine.UI;

/// Las monedas ganadas aparecen en el centro de la pantalla y vuelan en curva hasta el contador,
/// cada vez mas rapido. El saldo ya esta acreditado; el contador las va sumando recien cuando llegan.
/// Va en un RectTransform que cubre la pantalla, ultimo en el Canvas para dibujarse arriba de todo.
public class MonedasVoladoras : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CurrencyLabel contador;
    // El icono de la moneda del contador: ahi terminan el vuelo.
    [SerializeField] private RectTransform destino;
    [SerializeField] private Sprite spriteMoneda;

    [Header("Monedas")]
    // Se ganen 5 o 50, vuelan como mucho estas: cada una lleva una parte del premio.
    [SerializeField] private int maximoMonedas = 10;
    [SerializeField] private Vector2 tamanoMoneda = new Vector2(90f, 90f);

    [Header("Movimiento")]
    [SerializeField] private float radioAparicion = 160f;
    [SerializeField] private float duracionAparicion = 0.25f;
    [SerializeField] private float pausa = 0.2f;
    [SerializeField] private float separacion = 0.07f;
    [SerializeField] private float duracionVuelo = 0.6f;
    // Cuanto se aparta la curva de la linea recta, en unidades del Canvas.
    [SerializeField] private float curva = 350f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoMoneda;

    private RectTransform contenedor;
    private RectTransform[] monedas;
    private Vector2[] aparicion;
    private float[] desvio;
    private int[] valor;
    private float inicio;
    private int enVuelo;

    private void Awake()
    {
        contenedor = (RectTransform)transform;

        // Pool: se crean todas una vez y se prenden y apagan. Nada se instancia durante el efecto.
        monedas = new RectTransform[maximoMonedas];
        aparicion = new Vector2[maximoMonedas];
        desvio = new float[maximoMonedas];
        valor = new int[maximoMonedas];
        for (int i = 0; i < maximoMonedas; i++)
        {
            GameObject moneda = new GameObject("Moneda", typeof(RectTransform), typeof(Image));
            Image imagen = moneda.GetComponent<Image>();
            imagen.sprite = spriteMoneda;
            // Si no, las monedas se comerian los toques de los botones que tienen abajo.
            imagen.raycastTarget = false;
            monedas[i] = (RectTransform)moneda.transform;
            monedas[i].SetParent(contenedor, false);
            monedas[i].sizeDelta = tamanoMoneda;
            moneda.SetActive(false);
        }
    }

    // Hay que llamarlo ANTES de acreditar el saldo, asi el contador ya sabe que esas monedas
    // todavia no llegaron y no las cuenta de una.
    public void Lanzar(int cantidad)
    {
        if (cantidad <= 0) return;

        // Si quedaba un vuelo anterior, lo que llevaba entra de una.
        for (int i = 0; i < enVuelo; i++) Llegar(i, false);

        enVuelo = Mathf.Min(cantidad, maximoMonedas);
        contador.Retener(cantidad);
        inicio = Time.unscaledTime;

        for (int i = 0; i < enVuelo; i++)
        {
            // Todas llevan lo mismo y el resto se reparte de a uno entre las primeras: la suma da justo
            // el premio.
            valor[i] = cantidad / enVuelo + (i < cantidad % enVuelo ? 1 : 0);
            aparicion[i] = Random.insideUnitCircle * radioAparicion;
            desvio[i] = Random.Range(0.5f, 1.2f);
            monedas[i].localPosition = Vector2.zero;
            monedas[i].localScale = Vector3.zero;
            monedas[i].gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (enVuelo == 0) return;

        // Se recalcula en cada frame: si el Safe Area o el layout mueven el contador, las monedas lo siguen.
        Vector2 fin = contenedor.InverseTransformPoint(destino.position);
        float t = Time.unscaledTime - inicio;
        bool quedan = false;

        for (int i = 0; i < enVuelo; i++)
        {
            if (!monedas[i].gameObject.activeSelf) continue;
            quedan = true;

            if (t < duracionAparicion)
            {
                // Salen del centro y se abren, frenando (ease out): el "pop" de aparecer.
                float p = 1f - (1f - t / duracionAparicion) * (1f - t / duracionAparicion);
                monedas[i].localPosition = Vector2.Lerp(Vector2.zero, aparicion[i], p);
                monedas[i].localScale = Vector3.one * p;
                continue;
            }

            float vuelo = (t - duracionAparicion - pausa - i * separacion) / duracionVuelo;
            if (vuelo <= 0f)
            {
                // Esperando su turno: quieta donde aparecio (el ultimo frame de aparecer puede no
                // haber llegado justo al final).
                monedas[i].localPosition = aparicion[i];
                monedas[i].localScale = Vector3.one;
                continue;
            }
            if (vuelo >= 1f)
            {
                Llegar(i, true);
                continue;
            }

            // Progreso al cuadrado: arranca lento y acelera, como si el contador las atrajera.
            float q = vuelo * vuelo;
            Vector2 desde = aparicion[i];
            Vector2 medio = (desde + fin) * 0.5f;
            Vector2 costado = new Vector2(-(fin - desde).y, (fin - desde).x).normalized;
            Vector2 control = medio + costado * curva * desvio[i];
            // Bezier cuadratica: la moneda "tira" hacia el punto de control y dibuja una curva.
            monedas[i].localPosition = (1 - q) * (1 - q) * desde + 2 * (1 - q) * q * control + q * q * fin;
            monedas[i].localScale = Vector3.one * Mathf.Lerp(1f, 0.6f, q);
        }

        if (!quedan) enVuelo = 0;
    }

    private void Llegar(int i, bool conSonido)
    {
        if (!monedas[i].gameObject.activeSelf) return;
        monedas[i].gameObject.SetActive(false);
        contador.Soltar(valor[i]);
        if (conSonido) SoundPlayer.Reproducir(sonidoMoneda);
    }
}
