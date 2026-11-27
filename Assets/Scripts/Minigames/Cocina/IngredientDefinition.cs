using UnityEngine;

// ============================================================================
// IngredientDefinition.cs  (minijuego "Cocina")
// QUE HACE: describe un ingrediente: id, nombre, dibujo y precio.
// DONDE ESTA: un asset por ingrediente en Data/Cocina (Ing_Harina, Ing_Huevo...).
// PARA CAMBIAR: el precio en la tienda -> "Precio" del asset. Ojo: el precio
//               tambien suma a la energia que dan los platos que lo usan.
// PARA UNO NUEVO: Create > TalkinPoku > Ingrediente, y sumarlo a la tienda
//                 (TiendaComida) y al estante de la escena Cocina.
// ============================================================================

// Un ingrediente de la cocina. Lo que se compara (y lo que se guardaria) es el id: renombrar el
// asset o cambiarle el dibujo no rompe ninguna receta.
[CreateAssetMenu(menuName = "TalkinPoku/Ingrediente", fileName = "Ing_Nuevo")]
public class IngredientDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string nombre;
    [SerializeField] private Sprite sprite;
    // [AJUSTE] Precio en monedas.
    // Lo que cuesta en la tienda, en monedas. Tambien es lo que aporta a la energia de los platos.
    [SerializeField] private int precio = 10;

    public string Id => id;
    public string Nombre => nombre;
    public Sprite Sprite => sprite;
    public int Precio => precio;
}
