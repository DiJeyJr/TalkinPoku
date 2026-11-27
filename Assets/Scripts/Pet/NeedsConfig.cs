using UnityEngine;

// ============================================================================
// NeedsConfig.cs
// QUE HACE: guarda todos los numeros de las necesidades de Poku. Es un asset
//           (ScriptableObject) para poder balancear sin tocar codigo.
// DONDE ESTA: Data/NeedsConfig.asset. Se edita seleccionando ese asset y
//             cambiando los valores en el Inspector.
// OJO: los numeros escritos abajo son solo los valores por defecto al crear el
//      asset. El valor que usa el juego es el que figura en el Inspector de
//      NeedsConfig.asset. Para cambiar un numero, cambiarlo ahi.
// QUE SE PUEDE CAMBIAR (todos en el Inspector del asset):
//   - con cuanto arranca cada necesidad -> Valor Inicial
//   - cuando Poku se pone triste / critico -> Umbral Bajo / Umbral Critico
//   - cuanto baja cada necesidad por hora -> Hambre/Higiene/Diversion/Energia Por Hora
//   - cuanta energia recupera durmiendo -> Energia Durmiendo Por Hora
//   - bonus de la Mega en los cuidados -> Multiplicador Mega
//   - cuanto se descuenta como maximo con la app cerrada -> Horas Maximas Ausente
//   - manchas de suciedad -> Higiene Sin Manchas / Manchas Maximas
// ============================================================================

// Numeros del sistema de necesidades. Van en un asset y no en el script para poder balancear
// sin tocar codigo.
[CreateAssetMenu(menuName = "TalkinPoku/Needs Config", fileName = "NeedsConfig")]
public class NeedsConfig : ScriptableObject
{
    [Header("Valores")]
    // [AJUSTE] Valor de cada necesidad en una partida nueva (0 a 100).
    [SerializeField] private float valorInicial = 80f;
    // [AJUSTE] Umbrales de la cara y los avisos.
    // Debajo de este valor la necesidad ya se nota en Poku: le cambia la cara y aparecen manchas.
    [SerializeField] private float umbralBajo = 50f;
    [SerializeField] private float umbralCritico = 20f;

    [Header("Cuanto baja por hora")]
    // [AJUSTE] Puntos que pierde cada necesidad por hora real.
    [SerializeField] private float hambrePorHora = 8f;
    [SerializeField] private float higienePorHora = 5f;
    [SerializeField] private float diversionPorHora = 6f;
    [SerializeField] private float energiaPorHora = 4f;

    [Header("Dormir")]
    // [AJUSTE] Puntos de energia que recupera por hora durmiendo.
    [SerializeField] private float energiaDurmiendoPorHora = 25f;
    // [AJUSTE] Fraccion a la que bajan las otras necesidades mientras duerme (0.5 = la mitad).
    // Durmiendo, hambre, higiene y diversion bajan a esta fraccion de lo normal.
    [SerializeField] private float factorDormido = 0.5f;

    [Header("Mega Evolucion")]
    // [AJUSTE] Multiplicador de los cuidados con la Mega activa (2 = el doble).
    // Con Mega-Pokuchan todo lo que lo cuida rinde mas: la comida llena mas hambre, jugar mas
    // diversion y dormido recupera energia mas rapido. Es parte del premio de activarla.
    [SerializeField] private float multiplicadorMega = 2f;

    [Header("App cerrada")]
    // [AJUSTE] Horas maximas que se descuentan al volver a abrir la app.
    // Lo maximo que se descuenta al volver. Sin tope, despues de unos dias Poku estaria en cero.
    [SerializeField] private float horasMaximasAusente = 6f;

    [Header("Higiene por manchas")]
    // [AJUSTE] Desde que higiene aparecen manchas y cuantas como maximo.
    // Por dentro la higiene baja de a poco como las demas. Mientras este por encima de este valor
    // Poku esta limpio; debajo aparece una mancha por cada tramo, hasta el maximo.
    [SerializeField] private float higieneSinManchas = 50f;
    [SerializeField] private int manchasMaximas = 5;

    // Propiedades de solo lectura que usa NeedsSystem.
    public float ValorInicial => valorInicial;
    public float HigieneSinManchas => higieneSinManchas;
    public int ManchasMaximas => manchasMaximas;
    public float UmbralBajo => umbralBajo;
    public float UmbralCritico => umbralCritico;
    public float EnergiaDurmiendoPorHora => energiaDurmiendoPorHora;
    public float FactorDormido => factorDormido;
    public float HorasMaximasAusente => horasMaximasAusente;
    public float MultiplicadorMega => multiplicadorMega;

    // Devuelve cuanto baja por hora una necesidad.
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
