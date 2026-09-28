using UnityEngine;

/// Esponja del bano: frotarla sobre las manchas de Poku las va borrando. Cuenta la distancia
/// recorrida encima y no el tiempo, para que dejarla quieta no limpie.
[RequireComponent(typeof(Draggable))]
public class Sponge : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PetStains manchas;

    [Header("Umbrales")]
    // Radio de la esponja en unidades de mundo: una mancha cuenta apenas la esponja la roza.
    [SerializeField] private float alcance = 0.6f;

    private Draggable draggable;
    private Vector2 ultimoPunto;
    private bool hayUltimoPunto;

    private void Awake()
    {
        draggable = GetComponent<Draggable>();
    }

    private void OnEnable()
    {
        draggable.Levantado += AlLevantar;
        draggable.Movido += AlMover;
    }

    private void OnDisable()
    {
        draggable.Levantado -= AlLevantar;
        draggable.Movido -= AlMover;
    }

    private void AlLevantar()
    {
        // Cada arrastre empieza de cero: si no, el primer tramo iria desde donde quedo la anterior.
        hayUltimoPunto = false;
    }

    private void AlMover(Vector2 puntoMundo)
    {
        if (manchas == null) return;

        // El centro de la esponja y no el dedo: es lo que el jugador ve encima de las manchas.
        Vector2 punto = transform.position;
        if (hayUltimoPunto) manchas.Frotar(punto, alcance, Vector2.Distance(ultimoPunto, punto));

        ultimoPunto = punto;
        hayUltimoPunto = true;
    }
}
