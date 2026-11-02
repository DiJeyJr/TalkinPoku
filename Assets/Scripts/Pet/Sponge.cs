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

    [Header("Feedback")]
    // Opcional: sin sistema de particulas la esponja limpia igual.
    [SerializeField] private ParticleSystem burbujas;
    // Por distancia y no por frame: frotar rapido hace mas espuma que frotar despacio.
    [SerializeField] private float burbujasPorUnidad = 12f;

    private Draggable draggable;
    private Stain[] manchasHijas;
    private float burbujasPendientes;
    private Vector2 ultimoPunto;
    private bool hayUltimoPunto;

    private void Awake()
    {
        draggable = GetComponent<Draggable>();
        // Las manchas son hijas de PetStains. Se buscan una vez para saber si la esponja esta encima de alguna.
        if (manchas != null) manchasHijas = manchas.GetComponentsInChildren<Stain>(true);
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
        if (hayUltimoPunto)
        {
            float distancia = Vector2.Distance(ultimoPunto, punto);
            // Se pregunta antes de frotar: la ultima pasada apaga la mancha y igual tiene que hacer espuma.
            if (burbujas != null && TocaAlgunaMancha(punto)) Burbujear(distancia);
            manchas.Frotar(punto, alcance, distancia);
        }

        ultimoPunto = punto;
        hayUltimoPunto = true;
    }

    private bool TocaAlgunaMancha(Vector2 punto)
    {
        foreach (Stain mancha in manchasHijas)
        {
            if (mancha.Activa && mancha.Toca(punto, alcance)) return true;
        }
        return false;
    }

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
