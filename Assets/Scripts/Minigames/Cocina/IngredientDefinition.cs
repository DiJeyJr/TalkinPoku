using UnityEngine;

/// Un ingrediente de la cocina. Lo que se compara (y lo que se guardaria) es el id: renombrar el
/// asset o cambiarle el dibujo no rompe ninguna receta.
[CreateAssetMenu(menuName = "TalkinPoku/Ingrediente", fileName = "Ing_Nuevo")]
public class IngredientDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string nombre;
    [SerializeField] private Sprite sprite;

    public string Id => id;
    public string Nombre => nombre;
    public Sprite Sprite => sprite;
}
