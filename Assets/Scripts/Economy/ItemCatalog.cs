using System.Collections.Generic;
using UnityEngine;

/// Todos los objetos que existen. La tienda arma sus estantes con esta lista y el inventario la usa
/// para volver de un id del guardado al asset.
[CreateAssetMenu(menuName = "TalkinPoku/Catalogo de items", fileName = "ItemCatalog")]
public class ItemCatalog : ScriptableObject
{
    // En el orden en que aparecen en la tienda.
    [SerializeField] private ItemDefinition[] items;

    public IReadOnlyList<ItemDefinition> Items => items;

    public ItemDefinition Buscar(string id)
    {
        foreach (ItemDefinition item in items)
        {
            if (item != null && item.Id == id) return item;
        }
        return null;
    }
}
