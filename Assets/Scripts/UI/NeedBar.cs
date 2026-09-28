using UnityEngine;
using UnityEngine.UI;

/// Barra de una necesidad. La Image de relleno tiene que estar en Image Type = Filled.
public class NeedBar : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    [SerializeField] private Image relleno;
    [SerializeField] private NeedType tipo;

    [Header("Colores")]
    [SerializeField] private Color colorNormal = new Color(0.3f, 0.8f, 0.4f);
    [SerializeField] private Color colorCritico = new Color(0.9f, 0.25f, 0.2f);

    [Header("Feedback")]
    // Opcional. Late si la necesidad sube de golpe (comer, limpiar una mancha, un premio): el
    // relleno solo cambia unos pixeles y no se nota.
    [SerializeField] private Latido latido;
    [SerializeField] private float subidaParaLatir = 3f;

    // Hasta pintar la primera vez no hay con que comparar: al arrancar ningun cambio cuenta como subida.
    private float ultimo = float.MaxValue;

    private void OnEnable()
    {
        if (necesidades != null) necesidades.NeedChanged += AlCambiar;
    }

    private void OnDisable()
    {
        if (necesidades != null) necesidades.NeedChanged -= AlCambiar;
    }

    private void Start()
    {
        if (necesidades != null) Pintar(necesidades.Obtener(tipo));
    }

    private void AlCambiar(NeedType cambiada, float valor)
    {
        if (cambiada != tipo) return;
        // El decaimiento baja de a poco cada frame: solo una subida grande de una vez es "de golpe".
        if (latido != null && valor - ultimo >= subidaParaLatir) latido.Latir();
        Pintar(valor);
    }

    private void Pintar(float valor)
    {
        ultimo = valor;
        relleno.fillAmount = valor / 100f;
        relleno.color = valor < necesidades.UmbralCritico ? colorCritico : colorNormal;
    }
}
