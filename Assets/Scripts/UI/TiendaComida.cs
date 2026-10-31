using UnityEngine;

/// La pestana Comida de la tienda: arma un producto por ingrediente, en el orden de la lista. Para
/// vender uno nuevo alcanza con crear su asset y sumarlo aca.
public class TiendaComida : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ProductoTienda productoPrefab;
    [SerializeField] private Transform grilla;
    [SerializeField] private Wallet billetera;

    [Header("Productos")]
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
