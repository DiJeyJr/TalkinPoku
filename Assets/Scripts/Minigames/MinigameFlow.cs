using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// Arranca el minijuego de la escena, muestra el resultado y vuelve a la casa. Es el mismo para
/// todos los minijuegos: busca el IMinigame que tiene al lado y no sabe cual es.
public class MinigameFlow : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string escenaCasa = "SampleScene";

    [Header("Resultado")]
    [SerializeField] private GameObject panelResultado;
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private TMP_Text textoMonedas;
    [SerializeField] private Button botonVolver;

    // La ultima partida terminada, hasta que la casa la cobre. Estatica porque al volver se carga
    // otra escena y este componente deja de existir.
    private static MinigameResult? resultadoPendiente;

    private IMinigame minijuego;

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
        // GetComponent acepta interfaces; un [SerializeField] de tipo IMinigame no se veria en el Inspector.
        minijuego = GetComponent<IMinigame>();
        minijuego.Finished += MostrarResultado;
        botonVolver.onClick.AddListener(VolverACasa);
        panelResultado.SetActive(false);
    }

    private void Start()
    {
        minijuego.StartGame();
    }

    private void MostrarResultado(MinigameResult resultado)
    {
        // El premio no se aplica aca sino en la casa (MinigameReward), que es donde viven las necesidades.
        resultadoPendiente = resultado;
        textoPuntos.SetText("Puntos: {0}", resultado.Score);
        textoMonedas.SetText("Monedas: {0}", resultado.Coins);
        panelResultado.SetActive(true);
    }

    private void VolverACasa()
    {
        SceneManager.LoadScene(escenaCasa);
    }
}
