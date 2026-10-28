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

    private void Start()
    {
        if (!MinigameFlow.TomarResultado(out MinigameResult resultado)) return;

        // Una partida abandonada no divierte ni paga.
        if (!resultado.Completed) return;

        // Se mira al cobrar y no al empezar la partida: si la Mega se termino mientras jugaba, ya no paga
        // de mas. Redondea para arriba para que una partida de pocas monedas tambien note el bonus.
        int monedas = resultado.Coins;
        if (MegaEvolution.Activa && progreso != null)
            monedas = Mathf.CeilToInt(monedas * progreso.Config.MultiplicadorMonedasMega);

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
