using UnityEngine;

// Un ingrediente de la cocina. Lo que se compara (y lo que se guardaria) es el id: renombrar el
// asset o cambiarle el dibujo no rompe ninguna receta.
[CreateAssetMenu(menuName = "TalkinPoku/Ingrediente", fileName = "Ing_Nuevo")]
public class IngredientDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string nombre;
    [SerializeField] private Sprite sprite;
    // Lo que cuesta en la tienda, en monedas. Tambien es lo que aporta a la energia de los platos.
    [SerializeField] private int precio = 10;

    public string Id => id;
    public string Nombre => nombre;
    public Sprite Sprite => sprite;
    public int Precio => precio;
}
