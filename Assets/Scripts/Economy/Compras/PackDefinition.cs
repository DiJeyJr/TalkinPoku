using UnityEngine;

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
    [SerializeField] private CurrencyType moneda = CurrencyType.Monedas;
    [SerializeField] private int cantidad = 300;

    [Header("Tienda")]
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
