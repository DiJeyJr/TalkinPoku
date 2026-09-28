using UnityEngine;

/// Numeros del sistema de necesidades. Van en un asset y no en el script para poder balancear
/// sin tocar codigo.
[CreateAssetMenu(menuName = "TalkinPoku/Needs Config", fileName = "NeedsConfig")]
public class NeedsConfig : ScriptableObject
{
    [Header("Valores")]
    [SerializeField] private float valorInicial = 80f;
    // Debajo de este valor la necesidad ya se nota en Poku: le cambia la cara y aparecen manchas.
    [SerializeField] private float umbralBajo = 50f;
    [SerializeField] private float umbralCritico = 20f;

    [Header("Cuanto baja por hora")]
    [SerializeField] private float hambrePorHora = 8f;
    [SerializeField] private float higienePorHora = 5f;
    [SerializeField] private float diversionPorHora = 6f;
    [SerializeField] private float energiaPorHora = 4f;

    [Header("Dormir")]
    [SerializeField] private float energiaDurmiendoPorHora = 25f;
    // Durmiendo, hambre, higiene y diversion bajan a esta fraccion de lo normal.
    [SerializeField] private float factorDormido = 0.5f;

    [Header("App cerrada")]
    // Lo maximo que se descuenta al volver. Sin tope, despues de unos dias Poku estaria en cero.
    [SerializeField] private float horasMaximasAusente = 6f;

    [Header("Higiene por manchas")]
    // Por dentro la higiene baja de a poco como las demas. Mientras este por encima de este valor
    // Poku esta limpio; debajo aparece una mancha por cada tramo, hasta el maximo.
    [SerializeField] private float higieneSinManchas = 50f;
    [SerializeField] private int manchasMaximas = 5;

    public float ValorInicial => valorInicial;
    public float HigieneSinManchas => higieneSinManchas;
    public int ManchasMaximas => manchasMaximas;
    public float UmbralBajo => umbralBajo;
    public float UmbralCritico => umbralCritico;
    public float EnergiaDurmiendoPorHora => energiaDurmiendoPorHora;
    public float FactorDormido => factorDormido;
    public float HorasMaximasAusente => horasMaximasAusente;

    public float DecaimientoPorHora(NeedType tipo)
    {
        switch (tipo)
        {
            case NeedType.Hambre: return hambrePorHora;
            case NeedType.Higiene: return higienePorHora;
            case NeedType.Diversion: return diversionPorHora;
            case NeedType.Energia: return energiaPorHora;
            default: return 0f;
        }
    }
}
