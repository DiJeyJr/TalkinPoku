using UnityEngine;

// ============================================================================
// FoodItem.cs
// QUE HACE: una comida que se arrastra hasta Poku. Si se suelta encima, Poku la
//           come y le sube la necesidad que diga el item.
// DONDE ESTA: en "Habitaciones/Cocina/Manzana" de SampleScene (usa Draggable).
// PARA CAMBIAR: cuanto llena -> "Cantidad" del asset del item (ej: Item_Manzana).
// ============================================================================

// Comida de la cocina: se arrastra hasta la mascota y al soltarla encima sube la necesidad que
// diga el ItemDefinition. Si se suelta en otro lado, Draggable la devuelve a su lugar.
[RequireComponent(typeof(Draggable))]
public class FoodItem : MonoBehaviour
{
    [Header("Referencias")]
    // El asset de la comida (de ahi salen la necesidad y la cantidad).
    [SerializeField] private ItemDefinition item;
    [SerializeField] private PetDropTarget mascota;

    private Draggable draggable;

    private void Awake()
    {
        draggable = GetComponent<Draggable>();
    }

    private void OnEnable()
    {
        draggable.Soltado += AlSoltar;
    }

    private void OnDisable()
    {
        draggable.Soltado -= AlSoltar;
    }

    // Al soltar: si cayo sobre Poku, se la da de comer.
    private void AlSoltar(Vector2 puntoMundo)
    {
        if (item == null || mascota == null) return;
        if (!mascota.Contiene(puntoMundo)) return;

        mascota.Comer(item.Necesidad, item.Cantidad);
    }
}
