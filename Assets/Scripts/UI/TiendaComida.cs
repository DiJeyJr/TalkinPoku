using UnityEngine;

// ============================================================================
// TiendaComida.cs
// QUE HACE: la pestana "Ingredientes" de la tienda. Crea un ProductoTienda por
//           cada ingrediente de la lista.
// DONDE ESTA: en "Canvas/Pantallas/Tienda/Contenido/Paneles/PanelIngredientes".
// PARA CAMBIAR: que ingredientes se venden y en que orden -> lista
//               "Ingredientes" en el Inspector. Para uno nuevo, crear su asset
//               (Create > TalkinPoku > Ingrediente) y sumarlo a la lista.
// ============================================================================

// La pestana Comida de la tienda: arma un producto por ingrediente, en el orden de la lista. Para
// vender uno nuevo alcanza con crear su asset y sumarlo aca.
public class TiendaComida : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ProductoTienda productoPrefab;
    [SerializeField] private Transform grilla;
    [SerializeField] private Wallet billetera;

    [Header("Productos")]
    // [AJUSTE] Ingredientes a la venta.
    [SerializeField] private IngredientDefinition[] ingredientes;

    // En Start y no en Awake: el panel arranca apagado y se arma recien la primera vez que se abre.
    private void Start()
    {
        foreach (IngredientDefinition ingrediente in ingredientes)
        {
            ProductoTienda producto = Instantiate(productoPrefab, grilla);
            producto.Configurar(ingrediente, billetera);
        }
    }
}
