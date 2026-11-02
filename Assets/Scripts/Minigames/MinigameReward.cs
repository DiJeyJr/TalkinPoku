using System.Collections;
using UnityEngine;

/// Cobra la ultima partida al volver a la casa: acredita las monedas, le da a Poku la diversion y
/// lo pone contento unos segundos. Va en la casa y no en el minijuego porque aca viven la billetera
/// y las necesidades, y es el unico lugar que paga: un solo lugar para balancear.
public class MinigameReward : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    [SerializeField] private PetMoodController cara;
    [SerializeField] private Wallet billetera;
    [SerializeField] private MonedasVoladoras monedasVoladoras;
    [SerializeField] private Progression progreso;

    [Header("Premio")]
    [SerializeField] private float diversionPorPartida = 25f;
    [SerializeField] private float segundosContento = 3f;

    // Cuanto va a cobrar esta partida. Lo usa tambien la pantalla de resultado, que no puede tener
    // la logica de pago propia: si se balancea el bonus, cambia en un solo lugar.
    // Se mira al cobrar y no al empezar la partida: si la Mega se termino mientras jugaba, ya no paga
    // de mas. Redondea para arriba para que una partida de pocas monedas tambien note el bonus.
    public static int MonedasCobradas(MinigameResult resultado, ProgressionConfig config)
    {
        // Una partida abandonada no divierte ni paga.
        if (!resultado.Completed) return 0;

        int monedas = resultado.Coins;
        if (MegaEvolution.Activa && config != null)
            monedas = Mathf.CeilToInt(monedas * config.MultiplicadorMonedasMega);
        return monedas;
    }

    public static int XpCobrada(MinigameResult resultado, ProgressionConfig config)
    {
        if (!resultado.Completed || config == null) return 0;
        return config.BuscarPremio(XpSource.Minijuego, out ProgressionConfig.Premio premio) ? premio.xp : 0;
    }

    private IEnumerator Start()
    {
        if (!MinigameFlow.TomarResultado(out MinigameResult resultado)) yield break;

        if (!resultado.Completed) yield break;

        // Si las monedas salen mientras la pantalla de carga tapa la casa, el jugador no las ve volar.
        while (LoadingScreen.Tapando) yield return null;

        int monedas = MonedasCobradas(resultado, progreso != null ? progreso.Config : null);

        // Primero el vuelo y despues el saldo: asi el contador sabe que esas monedas vienen volando.
        if (monedasVoladoras != null) monedasVoladoras.Lanzar(monedas);
        billetera.Sumar(CurrencyType.Monedas, monedas);
        necesidades.Sumar(NeedType.Diversion, diversionPorPartida);
        cara.Alegrar(segundosContento);
        if (progreso != null) progreso.Ganar(XpSource.Minijuego);

        // Las monedas no pueden depender del proximo autoguardado: si la app se cierra antes, se pierden.
        SaveSystem.Guardar();
    }
}
