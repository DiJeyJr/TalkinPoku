using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// Arranca el minijuego de la escena, muestra el resultado y vuelve a la casa. Es el mismo para
/// todos los minijuegos: busca el Minijuego que tiene al lado y no sabe cual es.
public class MinigameFlow : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string escenaCasa = "SampleScene";

    [Header("Resultado")]
    [SerializeField] private PantallaResultado pantalla;
    // Para abandonar a mitad de partida: si no, la unica salida es perder o terminar.
    [SerializeField] private Button botonSalir;

    // La ultima partida terminada, hasta que la casa la cobre. Estatica porque al volver se carga
    // otra escena y este componente deja de existir.
    private static MinigameResult? resultadoPendiente;

    private Minijuego minijuego;

    // La llama MinigameReward al arrancar la casa. Devuelve false si no hay partida para cobrar, y
    // la borra para que no se cobre dos veces.
    public static bool TomarResultado(out MinigameResult resultado)
    {
        resultado = resultadoPendiente.GetValueOrDefault();
        bool habia = resultadoPendiente.HasValue;
        resultadoPendiente = null;
        return habia;
    }

    private void Awake()
    {
        // Pidiendo la clase base, GetComponent encuentra cualquiera de los seis.
        minijuego = GetComponent<Minijuego>();
        minijuego.Finished += MostrarResultado;
        pantalla.PidioReintentar += Reintentar;
        pantalla.PidioVolver += VolverACasa;
        pantalla.Ocultar();
        // Es navegacion de la interfaz, no una accion del juego: por eso alcanza con el onClick.
        if (botonSalir != null) botonSalir.onClick.AddListener(VolverACasa);
    }

    private void Start()
    {
        minijuego.StartGame();
    }

    private void MostrarResultado(MinigameResult resultado)
    {
        // El premio no se aplica aca sino en la casa (MinigameReward), que es donde viven las necesidades.
        // Si ya habia una partida sin cobrar (se reintento sin pasar por la casa) se suman las monedas,
        // para que reintentar no tire a la basura lo ganado antes.
        if (resultadoPendiente.HasValue)
        {
            MinigameResult previo = resultadoPendiente.Value;
            resultadoPendiente = new MinigameResult(resultado.Score, previo.Coins + resultado.Coins,
                previo.Completed || resultado.Completed, resultado.Won);
        }
        else
        {
            resultadoPendiente = resultado;
        }

        // Se muestra lo acumulado y no la ultima partida: es lo que se va a cobrar en la casa.
        pantalla.Mostrar(resultadoPendiente.Value);
    }

    private void Reintentar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void VolverACasa()
    {
        SceneManager.LoadScene(escenaCasa);
    }
}
