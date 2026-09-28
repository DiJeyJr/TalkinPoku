using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// Una receta: que ingredientes lleva y que plato sale. Es un dato, no codigo: para sumar una
/// receta se crea otro asset y se agrega a la lista de CocinaGame.
[CreateAssetMenu(menuName = "TalkinPoku/Receta", fileName = "Receta_Nueva")]
public class RecipeDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string nombre;
    [SerializeField] private Sprite sprite;
    [SerializeField] private IngredientDefinition[] ingredientes;

    public string Id => id;
    public string Nombre => nombre;
    public Sprite Sprite => sprite;
    public IReadOnlyList<IngredientDefinition> Ingredientes => ingredientes;

    // La olla arma la suya con la misma funcion, asi comparar dos combinaciones es comparar dos strings.
    public string Clave => ClaveDe(ingredientes.Select(i => i.Id));

    public static string ClaveDe(IEnumerable<string> ids)
    {
        // Ordenados para que "huevo+harina" y "harina+huevo" den lo mismo: el GDD pide descubrir
        // que ingredientes van, no en que orden se tiran.
        return string.Join("+", ids.OrderBy(id => id));
    }
}
