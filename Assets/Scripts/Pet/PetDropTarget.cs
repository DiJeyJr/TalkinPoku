using UnityEngine;

// Va en la mascota. Las actividades le aplican efectos por aca, asi ninguna necesita conocer al
// NeedsSystem.
[RequireComponent(typeof(Collider2D))]
public class PetDropTarget : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;

    private Collider2D miCollider;

    private void Awake()
    {
        miCollider = GetComponent<Collider2D>();
    }

    public bool Contiene(Vector2 puntoMundo)
    {
        return miCollider.OverlapPoint(puntoMundo);
    }

    public void Aplicar(NeedType necesidad, float cantidad)
    {
        necesidades.Sumar(necesidad, cantidad);
    }
}
