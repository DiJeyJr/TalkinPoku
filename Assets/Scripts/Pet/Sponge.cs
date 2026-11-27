using UnityEngine;

// ============================================================================
// Sponge.cs
// QUE HACE: la esponja del bano. Al arrastrarla sobre las manchas de Poku las va
//           borrando y tira burbujas.
// DONDE ESTA: en "Habitaciones/Bano/Esponja" de SampleScene (usa Draggable).
// PARA CAMBIAR: que tan grande es el area que limpia -> alcance.
//               cuanta espuma sale -> burbujasPorUnidad.
//               cuanto hay que frotar cada mancha -> "Frotado Para Limpiar" en Stain.
// ============================================================================

// Esponja del bano: frotarla sobre las manchas de Poku las va borrando. Cuenta la distancia
// recorrida encima y no el tiempo, para que dejarla quieta no limpie.
[RequireComponent(typeof(Draggable))]
public class Sponge : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PetStains manchas;

    [Header("Umbrales")]
    // [AJUSTE] Radio de limpieza.
    // Radio de la esponja en unidades de mundo: una mancha cuenta apenas la esponja la roza.
    [SerializeField] private float alcance = 0.6f;

    [Header("Feedback")]
    // Opcional: sin sistema de particulas la esponja limpia igual.
    [SerializeField] private ParticleSystem burbujas;
    // [AJUSTE] Burbujas por unidad de distancia frotada.
    // Por distancia y no por frame: frotar rapido hace mas espuma que frotar despacio.
    [SerializeField] private float burbujasPorUnidad = 12f;

    private Draggable draggable;
    private float burbujasPendientes;
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

    // Mientras se mueve: mide cuanto se movio y frota las manchas que tiene abajo.
    private void AlMover(Vector2 puntoMundo)
    {
        if (manchas == null) return;

        // El centro de la esponja y no el dedo: es lo que el jugador ve encima de las manchas.
        Vector2 punto = transform.position;
        if (hayUltimoPunto)
        {
            float distancia = Vector2.Distance(ultimoPunto, punto);
            if (manchas.Frotar(punto, alcance, distancia) && burbujas != null) Burbujear(distancia);
        }

        ultimoPunto = punto;
        hayUltimoPunto = true;
    }

    // Tira burbujas segun la distancia frotada.
    private void Burbujear(float distancia)
    {
        // Se acumula el resto: con movimientos cortos daria menos de una burbuja por frame y nunca saldria ninguna.
        burbujasPendientes += distancia * burbujasPorUnidad;
        int cantidad = (int)burbujasPendientes;
        if (cantidad == 0) return;

        burbujasPendientes -= cantidad;
        burbujas.Emit(cantidad);
    }
}
