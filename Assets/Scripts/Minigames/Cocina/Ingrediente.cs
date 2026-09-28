using UnityEngine;

/// Un ingrediente del estante. Se arrastra con el mismo Draggable de la casa y, si se suelta sobre
/// la olla, entra. El del estante vuelve siempre a su lugar: hay de sobra.
[RequireComponent(typeof(Draggable))]
public class Ingrediente : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private IngredientDefinition definicion;
    [SerializeField] private Olla olla;

    private Draggable arrastre;

    private void Awake()
    {
        arrastre = GetComponent<Draggable>();

        // El dibujo sale del dato: si arte cambia el sprite del ingrediente, el estante se entera solo.
        SpriteRenderer dibujo = GetComponent<SpriteRenderer>();
        if (dibujo != null && definicion != null) dibujo.sprite = definicion.Sprite;
    }

    private void OnEnable()
    {
        arrastre.Soltado += AlSoltar;
    }

    private void OnDisable()
    {
        arrastre.Soltado -= AlSoltar;
    }

    private void AlSoltar(Vector2 puntoMundo)
    {
        if (olla.Contiene(puntoMundo)) olla.Agregar(definicion);
    }
}
