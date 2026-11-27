using TMPro;
using UnityEngine;

// ============================================================================
// CurrencyLabel.cs
// QUE HACE: el contador de monedas o gemas de la pantalla. Cuando cambia el
//           saldo no salta de golpe: va contando hasta el numero nuevo.
// DONDE ESTA: en "Canvas/SafeArea/HUD/Saldo/Monedas" y "Gemas", y en el saldo
//             de la tienda.
// PARA CAMBIAR: cuanto tarda en contar -> duracionConteo.
//               que moneda muestra -> "Tipo" en el Inspector.
// ============================================================================

// Muestra el saldo de una moneda. Cuando cambia no salta al numero nuevo: lo va contando en un rato
// corto, asi el jugador ve cuanto gano.
public class CurrencyLabel : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Wallet billetera;
    [SerializeField] private TMP_Text texto;

    [Header("Moneda")]
    [SerializeField] private CurrencyType tipo;

    [Header("Feedback")]
    // [AJUSTE] Segundos que tarda en contar hasta el saldo nuevo.
    [SerializeField] private float duracionConteo = 0.6f;
    // Opcional: late cada vez que entra una moneda que venia volando.
    [SerializeField] private Latido latido;

    // Numero que se ve ahora (con decimales mientras cuenta).
    private float mostrado;
    // Numero al que tiene que llegar.
    private int objetivo;
    private float velocidad;
    private int escrito = -1;
    // Monedas que ya estan en el saldo pero todavia vienen volando: se muestran cuando llegan.
    private int retenidas;

    private void OnEnable()
    {
        billetera.SaldoCambiado += AlCambiar;
        // Al prenderse (arranque, o se abre la tienda) muestra el saldo de una, sin contar.
        retenidas = 0;
        objetivo = billetera.Obtener(tipo);
        mostrado = objetivo;
        Escribir();
    }

    private void OnDisable()
    {
        billetera.SaldoCambiado -= AlCambiar;
    }

    // La llama MonedasVoladoras antes de acreditar el premio.
    public void Retener(int cantidad)
    {
        retenidas += cantidad;
    }

    // La llama MonedasVoladoras cuando una moneda llega al contador.
    public void Soltar(int cantidad)
    {
        retenidas = Mathf.Max(0, retenidas - cantidad);
        Contar(billetera.Obtener(tipo));
        if (latido != null) latido.Latir();
    }

    private void AlCambiar(CurrencyType cambiada, int saldo)
    {
        if (cambiada == tipo) Contar(saldo);
    }

    // Fija el nuevo objetivo y la velocidad para llegar en duracionConteo segundos.
    private void Contar(int saldo)
    {
        objetivo = saldo - retenidas;
        // Tarda siempre lo mismo, se ganen 5 o 500: la velocidad sale de la diferencia.
        velocidad = Mathf.Abs(objetivo - mostrado) / duracionConteo;
    }

    // Cada frame acerca el numero mostrado al objetivo.
    private void Update()
    {
        if (mostrado == objetivo) return;
        mostrado = Mathf.MoveTowards(mostrado, objetivo, velocidad * Time.deltaTime);
        Escribir();
    }

    private void Escribir()
    {
        // Solo se reescribe cuando cambia el entero, no en cada frame del conteo.
        int entero = Mathf.RoundToInt(mostrado);
        if (entero == escrito) return;
        escrito = entero;
        texto.SetText("{0}", entero);
    }
}
