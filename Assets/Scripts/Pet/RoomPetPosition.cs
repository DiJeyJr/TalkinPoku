using UnityEngine;

/// Va en una habitacion donde Poku tiene que pararse en otro lugar (en la sala de arte, mas abajo para
/// no tapar el cuadro). Al salir lo devuelve: la mascota es la misma en todas las habitaciones.
public class RoomPetPosition : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform mascota;

    [Header("Lugar")]
    [SerializeField] private Vector2 posicion = new Vector2(0f, -6f);

    private Vector3 original;

    private void OnEnable()
    {
        original = mascota.position;
        mascota.position = new Vector3(posicion.x, posicion.y, original.z);
    }

    private void OnDisable()
    {
        if (mascota != null) mascota.position = original;
    }
}
