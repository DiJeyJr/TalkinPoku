using UnityEngine;

// ============================================================================
// PackDefinition.cs
// QUE HACE: describe un pack de monedas o gemas que se compra con dinero real.
// DONDE ESTA: cada pack es un asset en Data/Packs (Pack_MonedasChico,
//             Pack_GemasGrande, etc).
// PARA CAMBIAR: cuanto trae -> "Cantidad" | de que moneda -> "Moneda"
//               precio que se muestra -> "Precio Vitrina". Todo en el asset.
// ============================================================================

// Un paquete de monedas o gemas que se compra con dinero real. El id es el nuestro y no cambia;
// el productId es el codigo con el que lo conoce la tienda (Google Play), y puede no coincidir.
[CreateAssetMenu(menuName = "TalkinPoku/Pack", fileName = "Pack_")]
public class PackDefinition : ScriptableObject
{
    [Header("Identidad")]
    [SerializeField] private string id;
    [SerializeField] private string nombre;
    [SerializeField] private Sprite icono;

    [Header("Contenido")]
    // [AJUSTE] Que moneda trae y cuanta.
    [SerializeField] private CurrencyType moneda = CurrencyType.Monedas;
    [SerializeField] private int cantidad = 300;

    [Header("Tienda")]
    // [AJUSTE] Texto del precio.
    // Solo para mostrar. Con la tienda real el precio lo manda Google Play, ya en la moneda del pais.
    [SerializeField] private string precioVitrina = "US$ 0,99";
    [SerializeField] private string productId;

    public string Id => id;
    public string Nombre => nombre;
    public Sprite Icono => icono;
    public CurrencyType Moneda => moneda;
    public int Cantidad => cantidad;
    public string PrecioVitrina => precioVitrina;
    public string ProductId => productId;
}
