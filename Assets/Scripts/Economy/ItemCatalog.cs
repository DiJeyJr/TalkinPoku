using System.Collections.Generic;
using UnityEngine;

// ============================================================================
// ItemCatalog.cs
// QUE HACE: la lista de TODOS los items del juego (comida, ropa y muebles).
// DONDE ESTA: es un asset: Data/Items/ItemCatalog.asset.
// PARA AGREGAR UN ITEM NUEVO A LA TIENDA:
//   1) Crear el asset: click derecho en Data/Items > Create > TalkinPoku > Item
//      (o duplicar uno parecido con Ctrl+D) y completar sus campos.
//   2) Arrastrarlo a la lista "Items" de ItemCatalog.asset.
//   Si no esta en esta lista, no aparece en la tienda.
// ============================================================================

// Todos los objetos que existen. La tienda arma sus estantes con esta lista y el inventario la usa
// para volver de un id del guardado al asset.
[CreateAssetMenu(menuName = "TalkinPoku/Catalogo de items", fileName = "ItemCatalog")]
public class ItemCatalog : ScriptableObject
{
    // [AJUSTE] Los items del juego.
    // En el orden en que aparecen en la tienda.
    [SerializeField] private ItemDefinition[] items;

    public IReadOnlyList<ItemDefinition> Items => items;

    // Busca un item por su id (el texto que va en la partida guardada). null si no existe.
    public ItemDefinition Buscar(string id)
    {
        foreach (ItemDefinition item in items)
        {
            if (item != null && item.Id == id) return item;
        }
        return null;
    }
}
