using System;
using UnityEngine;

// ============================================================================
// RoomNavigator.cs
// QUE HACE: maneja las habitaciones de la casa. Muestra una sola por vez y pasa
//           a la siguiente o a la anterior cuando el jugador desliza el dedo.
// DONDE ESTA: en el objeto "Sistemas" de SampleScene.
// PARA CAMBIAR: agregar, sacar o reordenar habitaciones -> la lista
//               "Habitaciones" en el Inspector (son los hijos de "Habitaciones":
//               0 Sala principal, 1 Cocina, 2 Bano, 3 Dormitorio, 4 Sala de
//               juegos, 5 Sala de arte). Si se agrega una, sumar tambien su
//               icono en RoomIndicator.
// AVISA: el evento HabitacionCambiada(indice) cada vez que cambia de cuarto.
// ============================================================================

// Muestra una habitacion por vez. Las habitaciones son hijos de la escena que se prenden y
// apagan, no escenas separadas: cargar una escena por cuarto da pantallas negras en mobile y
// ademas la mascota es la misma en todas.
public class RoomNavigator : MonoBehaviour
{
    [Header("Referencias")]
    // El que detecta el swipe (deslizar el dedo).
    [SerializeField] private SwipeDetector swipeDetector;
    // [AJUSTE] Las habitaciones, en el orden en que se recorren.
    [SerializeField] private GameObject[] habitaciones;

    // Indice de la habitacion que se ve ahora.
    private int actual;

    public int Actual => actual;
    public int Cantidad => habitaciones == null ? 0 : habitaciones.Length;

    // El nombre sale del GameObject para no mantener un segundo array de textos que se
    // desordena al reordenar las habitaciones.
    public string NombreActual => Cantidad > 0 && habitaciones[actual] != null ? habitaciones[actual].name : "";

    // Lo escuchan el indicador de habitaciones y otros scripts que necesitan saber en que cuarto se esta.
    public event Action<int> HabitacionCambiada;

    // Se suscribe al swipe al activarse y se desuscribe al apagarse (asi no queda escuchando de mas).
    private void OnEnable()
    {
        if (swipeDetector != null) swipeDetector.Swiped += AlDeslizar;
    }

    private void OnDisable()
    {
        if (swipeDetector != null) swipeDetector.Swiped -= AlDeslizar;
    }

    // Al arrancar muestra la primera habitacion (la Sala principal) y apaga las demas.
    private void Start()
    {
        Mostrar(actual);
    }

    private void AlDeslizar(SwipeDirection direccion)
    {
        // Deslizar hacia la izquierda trae la habitacion siguiente, como pasar una pagina.
        if (direccion == SwipeDirection.Izquierda) Siguiente();
        else Anterior();
    }

    // Avanza una habitacion. El % hace que despues de la ultima vuelva a la primera.
    private void Siguiente()
    {
        if (habitaciones == null || habitaciones.Length == 0) return;
        Mostrar((actual + 1) % habitaciones.Length);
    }

    // Retrocede una habitacion. Desde la primera va a la ultima.
    private void Anterior()
    {
        if (habitaciones == null || habitaciones.Length == 0) return;
        Mostrar((actual - 1 + habitaciones.Length) % habitaciones.Length);
    }

    // Para saltar directo desde los iconos del indicador, sin pasar por las del medio.
    public void IrA(int indice)
    {
        if (habitaciones == null || indice < 0 || indice >= habitaciones.Length) return;
        Mostrar(indice);
    }

    // Prende la habitacion pedida, apaga todas las demas y avisa el cambio.
    private void Mostrar(int indice)
    {
        if (habitaciones == null || habitaciones.Length == 0) return;

        for (int i = 0; i < habitaciones.Length; i++)
        {
            // Se desactiva el objeto en vez de taparlo: lo que esta apagado no se dibuja,
            // y en mobile el overdraw se paga por pixel.
            if (habitaciones[i] != null) habitaciones[i].SetActive(i == indice);
        }

        actual = indice;
        HabitacionCambiada?.Invoke(actual);
    }
}
