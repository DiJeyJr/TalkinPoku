using UnityEngine;

/// Esponja del bano: sube la higiene mientras se frota sobre la mascota. Cuenta la distancia
/// recorrida encima y no el tiempo, para que dejarla quieta no limpie.
[RequireComponent(typeof(Draggable))]
public class Sponge : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PetDropTarget mascota;

    [Header("Umbrales")]
    [SerializeField] private float higienePorUnidad = 4f;

    private Draggable draggable;
    private Vector2 ultimoPunto;
    private bool hayUltimoPunto;

    private void Awake()
    {
        draggable = GetComponent<Draggable>();
    }

    private void OnEnable()
    {
        draggable.Movido += AlMover;
        draggable.Soltado += AlSoltar;
    }

    private void OnDisable()
    {
        draggable.Movido -= AlMover;
        draggable.Soltado -= AlSoltar;
    }

    private void AlMover(Vector2 puntoMundo)
    {
        if (mascota == null) return;

        if (mascota.Contiene(puntoMundo))
        {
            if (hayUltimoPunto)
            {
                float distancia = Vector2.Distance(ultimoPunto, puntoMundo);
                mascota.Aplicar(NeedType.Higiene, distancia * higienePorUnidad);
            }
            ultimoPunto = puntoMundo;
            hayUltimoPunto = true;
        }
        else
        {
            // Al salir de la mascota se corta, asi entrar de nuevo no suma el tramo de afuera.
            hayUltimoPunto = false;
        }
    }

    private void AlSoltar(Vector2 puntoMundo)
    {
        hayUltimoPunto = false;
    }
}
