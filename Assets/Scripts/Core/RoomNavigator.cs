using System;
using UnityEngine;

/// Muestra una habitacion por vez. Las habitaciones son hijos de la escena que se prenden y
/// apagan, no escenas separadas: cargar una escena por cuarto da pantallas negras en mobile y
/// ademas la mascota es la misma en todas.
public class RoomNavigator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private SwipeDetector swipeDetector;
    [SerializeField] private GameObject[] habitaciones;

    private int actual;

    public int Actual => actual;
    public int Cantidad => habitaciones == null ? 0 : habitaciones.Length;

    // El nombre sale del GameObject para no mantener un segundo array de textos que se
    // desordena al reordenar las habitaciones.
    public string NombreActual => Cantidad > 0 && habitaciones[actual] != null ? habitaciones[actual].name : "";

    public event Action<int> HabitacionCambiada;

    private void OnEnable()
    {
        if (swipeDetector != null) swipeDetector.Swiped += AlDeslizar;
    }

    private void OnDisable()
    {
        if (swipeDetector != null) swipeDetector.Swiped -= AlDeslizar;
    }

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

    public void Siguiente()
    {
        if (habitaciones == null || habitaciones.Length == 0) return;
        Mostrar((actual + 1) % habitaciones.Length);
    }

    public void Anterior()
    {
        if (habitaciones == null || habitaciones.Length == 0) return;
        Mostrar((actual - 1 + habitaciones.Length) % habitaciones.Length);
    }

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
