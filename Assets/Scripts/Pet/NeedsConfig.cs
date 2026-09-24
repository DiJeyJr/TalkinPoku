using UnityEngine;

/// Numeros del sistema de necesidades. Van en un asset y no en el script para poder balancear
/// sin tocar codigo.
[CreateAssetMenu(menuName = "TalkinPoku/Needs Config", fileName = "NeedsConfig")]
public class NeedsConfig : ScriptableObject
{
    [Header("Valores")]
    [SerializeField] private float valorInicial = 80f;
    [SerializeField] private float umbralCritico = 20f;

    [Header("Cuanto baja por hora")]
    [SerializeField] private float hambrePorHora = 8f;
    [SerializeField] private float higienePorHora = 5f;
    [SerializeField] private float diversionPorHora = 6f;
    [SerializeField] private float energiaPorHora = 4f;

    [Header("Dormir")]
    [SerializeField] private float energiaDurmiendoPorHora = 25f;

    public float ValorInicial => valorInicial;
    public float UmbralCritico => umbralCritico;
    public float EnergiaDurmiendoPorHora => energiaDurmiendoPorHora;

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
