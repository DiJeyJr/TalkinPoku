using TMPro;
using UnityEngine;

/// Panel para probar el juego sin esperar horas: cambia la velocidad del tiempo, y sus filas
/// (AdminNeedRow) suben o bajan cada necesidad. No es parte del juego: se abre con AdminTrigger.
public class AdminPanel : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    [SerializeField] private ScreenStack pantallas;
    [SerializeField] private GameObject contenido;
    [SerializeField] private TMP_Text textoVelocidad;

    // Estatico para que sobreviva al ir a un minijuego: la casa se recarga y el panel con ella.
    // En 0 nadie la toco todavia y manda la del Inspector de NeedsSystem.
    private static float velocidadElegida;

    private void Start()
    {
        if (velocidadElegida > 0f) necesidades.MultiplicadorTiempo = velocidadElegida;
        MostrarVelocidad();
    }

    public void Alternar()
    {
        // Se abre como una pantalla mas: el ScreenStack apaga el input de la casa, y un toque en un
        // boton del panel no le llega tambien a Poku o a la esponja que estan abajo.
        if (contenido.activeSelf) pantallas.Cerrar();
        else pantallas.Abrir(contenido);
    }

    // La llaman los botones x1, x60, x600 y x3600 desde su OnClick, cada uno con su numero.
    public void CambiarVelocidad(float multiplicador)
    {
        velocidadElegida = multiplicador;
        necesidades.MultiplicadorTiempo = multiplicador;
        MostrarVelocidad();
    }

    private void MostrarVelocidad()
    {
        textoVelocidad.SetText("Velocidad x{0}", necesidades.MultiplicadorTiempo);
    }
}
