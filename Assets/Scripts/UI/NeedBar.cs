using UnityEngine;
using UnityEngine.UI;

// ============================================================================
// NeedBar.cs
// QUE HACE: la barra de una necesidad en el HUD (hambre, higiene, diversion,
//           energia o felicidad). Se llena segun el valor y se pone roja si
//           esta por debajo del umbral critico.
// DONDE ESTA: en "Canvas/SafeArea/HUD/Necesidades/BarraHambre", "BarraHigiene",
//             etc. de SampleScene.
// PARA CAMBIAR: colores -> colorNormal / colorCritico.
//               desde que valor se pone roja -> "Umbral Critico" en NeedsConfig.asset.
//               cuanto tiene que subir de golpe para latir -> subidaParaLatir.
// ============================================================================

// Barra de una necesidad. La Image de relleno tiene que estar en Image Type = Filled.
public class NeedBar : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    [SerializeField] private Image relleno;
    // Que necesidad muestra esta barra.
    [SerializeField] private NeedType tipo;

    [Header("Colores")]
    // [AJUSTE] Color normal y color cuando esta critica.
    [SerializeField] private Color colorNormal = new Color(0.3f, 0.8f, 0.4f);
    [SerializeField] private Color colorCritico = new Color(0.9f, 0.25f, 0.2f);

    [Header("Feedback")]
    // Opcional. Late si la necesidad sube de golpe (comer, limpiar una mancha, un premio): el
    // relleno solo cambia unos pixeles y no se nota.
    [SerializeField] private Latido latido;
    // [AJUSTE] Puntos que tiene que subir de una vez para que la barra lata.
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

    // Cambio una necesidad: si es la de esta barra, la repinta (y late si subio mucho).
    private void AlCambiar(NeedType cambiada, float valor)
    {
        if (cambiada != tipo) return;
        // El decaimiento baja de a poco cada frame: solo una subida grande de una vez es "de golpe".
        if (latido != null && valor - ultimo >= subidaParaLatir) latido.Latir();
        Pintar(valor);
    }

    // Relleno = valor / 100; color rojo si esta debajo del umbral critico.
    private void Pintar(float valor)
    {
        ultimo = valor;
        relleno.fillAmount = valor / NeedsSystem.Maximo;
        relleno.color = valor < necesidades.UmbralCritico ? colorCritico : colorNormal;
    }
}
