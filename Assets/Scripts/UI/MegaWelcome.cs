using System.Collections;
using UnityEngine;

// ============================================================================
// MegaWelcome.cs
// QUE HACE: el cartel de felicitaciones la primera vez que Pokuchan se
//           convierte en Mega-Pokuchan. Explica los bonus de la Mega. Sale una
//           sola vez por partida (MegaEvolution.Presentada queda guardado).
// DONDE ESTA: en "Canvas/DialogoMega" de SampleScene.
// PARA CAMBIAR: el texto del cartel -> Texto(). Los numeros salen solos de
//               ProgressionConfig y NeedsConfig.
//               cuanto espera antes de mostrarse -> segundosDeEspera.
// ============================================================================

// El cartel de la primera Mega Evolucion: felicita por llegar al maximo y explica que se gana
// mientras dura. Sale una sola vez por partida. Los numeros salen de los configs, asi el texto
// sigue siendo cierto si se rebalancea.
public class MegaWelcome : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MegaEvolution mega;
    [SerializeField] private Progression progreso;
    [SerializeField] private NeedsConfig necesidades;
    [SerializeField] private ConfirmDialog dialogo;

    [Header("Tiempos")]
    // [AJUSTE] Segundos de espera antes de mostrar el cartel.
    // Espera a que termine el pop de la transformacion: si sale enseguida, el cartel la tapa.
    [SerializeField] private float segundosDeEspera = 1.5f;

    private void OnEnable()
    {
        mega.PrimeraVez += AlPrimeraVez;
    }

    private void OnDisable()
    {
        mega.PrimeraVez -= AlPrimeraVez;
    }

    private void AlPrimeraVez()
    {
        StartCoroutine(MostrarDespues());
    }

    // Corrutina: espera y muestra el cartel.
    private IEnumerator MostrarDespues()
    {
        yield return new WaitForSeconds(segundosDeEspera);
        dialogo.Avisar(Texto());
    }

    // Arma el texto con los numeros reales de los configs.
    private string Texto()
    {
        ProgressionConfig config = progreso.Config;
        int masMonedas = Mathf.RoundToInt((config.MultiplicadorMonedasMega - 1f) * 100f);

        return "<size=125%><b>¡Felicitaciones!</b></size>\n"
            + "Ayudaste a Pokuchan a llegar a su máximo nivel y se convirtió en <b>Mega-Pokuchan</b>.\n\n"
            + "<align=left>Mientras dure la Mega:\n"
            + "• Ganás " + masMonedas + "% más monedas en los minijuegos.\n"
            + "• La comida, los juegos y las caricias rinden " + Veces(necesidades.MultiplicadorMega) + ".\n"
            + "• Durmiendo recupera energía " + Veces(necesidades.MultiplicadorMega) + " de rápido.</align>\n\n"
            + "<size=85%>Dura " + Mathf.RoundToInt(config.MinutosMega) + " minutos. ¡Cuidalo para volver a cargarla!</size>";
    }

    // 2 -> "el doble", 3 -> "el triple", otro -> "N veces más".
    private static string Veces(float multiplicador)
    {
        if (Mathf.Approximately(multiplicador, 2f)) return "el doble";
        if (Mathf.Approximately(multiplicador, 3f)) return "el triple";
        return multiplicador.ToString("0.#") + " veces más";
    }
}
