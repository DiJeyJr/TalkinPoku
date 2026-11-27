using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ============================================================================
// RecipeDefinition.cs  (minijuego "Cocina")
// QUE HACE: describe una receta: que ingredientes lleva, que plato sale y cuanta
//           energia le da a Poku.
// DONDE ESTA: un asset por receta en Data/Cocina (Receta_Ensalada, Receta_Flan...).
// PARA CAMBIAR: ingredientes -> lista "Ingredientes" del asset.
//               energia del plato -> suma de precios de los ingredientes +
//               "Energia Extra" del asset.
// PARA UNA NUEVA: Create > TalkinPoku > Receta, y sumarla a la lista "Recetas"
//                 de CocinaGame y de la Mesada (casa).
// ============================================================================

// Una receta: que ingredientes lleva y que plato sale. Es un dato, no codigo: para sumar una
// receta se crea otro asset y se agrega a la lista de CocinaGame.
[CreateAssetMenu(menuName = "TalkinPoku/Receta", fileName = "Receta_Nueva")]
public class RecipeDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string nombre;
    [SerializeField] private Sprite sprite;
    [SerializeField] private IngredientDefinition[] ingredientes;
    // [AJUSTE] Energia extra del plato.
    // Lo que el plato suma por si mismo, aparte de lo que valen sus ingredientes.
    [SerializeField] private int energiaExtra = 10;

    public string Id => id;
    public string Nombre => nombre;
    public Sprite Sprite => sprite;
    public IReadOnlyList<IngredientDefinition> Ingredientes => ingredientes;

    // La energia que le da a Poku: un plato con ingredientes mas caros llena mas, y el extra deja
    // ajustar cada receta sin tocar los precios de la tienda.
    public int Energia => ingredientes.Sum(i => i.Precio) + energiaExtra;

    // La olla arma la suya con la misma funcion, asi comparar dos combinaciones es comparar dos strings.
    public string Clave => ClaveDe(ingredientes.Select(i => i.Id));

    // Une los ids ordenados con "+" (ej: "harina+huevo+leche").
    public static string ClaveDe(IEnumerable<string> ids)
    {
        // Ordenados para que "huevo+harina" y "harina+huevo" den lo mismo: el GDD pide descubrir
        // que ingredientes van, no en que orden se tiran.
        return string.Join("+", ids.OrderBy(id => id));
    }
}
