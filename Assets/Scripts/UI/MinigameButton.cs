using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// Un boton de la seleccion de minijuegos. Los que todavia no existen quedan grises pero visibles,
/// asi el jugador ve que hay seis aunque no pueda entrar a todos.
[RequireComponent(typeof(Button))]
public class MinigameButton : MonoBehaviour
{
    [Header("Minijuego")]
    [SerializeField] private string escena;
    [SerializeField] private bool disponible;

    private Button boton;

    private void Awake()
    {
        boton = GetComponent<Button>();
        boton.interactable = disponible;
        boton.onClick.AddListener(Jugar);
    }

    private void Jugar()
    {
        // Mientras no exista la escena del minijuego el boton responde igual, para probar el flujo.
        if (string.IsNullOrEmpty(escena))
        {
            Debug.Log($"{name}: todavia no tiene escena asignada");
            return;
        }

        SceneManager.LoadScene(escena);
    }
}
