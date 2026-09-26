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

    private IMinigame minijuego;

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
        // Todavia no hay Wallet: cuando exista, este es el unico lugar que acredita las monedas.
        textoPuntos.SetText("Puntos: {0}", resultado.Score);
        textoMonedas.SetText("Monedas: {0}", resultado.Coins);
        panelResultado.SetActive(true);
    }

    private void VolverACasa()
    {
        SceneManager.LoadScene(escenaCasa);
    }
}
