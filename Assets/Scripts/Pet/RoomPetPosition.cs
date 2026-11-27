using UnityEngine;

// ============================================================================
// RoomPetPosition.cs
// QUE HACE: mueve a Poku a otra posicion mientras se esta en esta habitacion, y
//           lo devuelve a su lugar al salir.
// DONDE ESTA: en "Habitaciones/Sala de arte" de SampleScene (para que Poku no
//             tape el cuadro).
// PARA CAMBIAR: donde se para Poku en esa habitacion -> "Posicion" (Inspector).
//               Para usarlo en otra habitacion, agregar este componente a ella.
// ============================================================================

// Va en una habitacion donde Poku tiene que pararse en otro lugar (en la sala de arte, mas abajo para
// no tapar el cuadro). Al salir lo devuelve: la mascota es la misma en todas las habitaciones.
public class RoomPetPosition : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform mascota;

    [Header("Lugar")]
    // [AJUSTE] Posicion de Poku en esta habitacion (unidades de mundo).
    [SerializeField] private Vector2 posicion = new Vector2(0f, -6f);

    // Donde estaba antes de entrar.
    private Vector3 original;

    // Al entrar a la habitacion (se prende): guarda la posicion y lo mueve.
    private void OnEnable()
    {
        original = mascota.position;
        mascota.position = new Vector3(posicion.x, posicion.y, original.z);
    }

    // Al salir (se apaga): lo devuelve.
    private void OnDisable()
    {
        if (mascota != null) mascota.position = original;
    }
}
