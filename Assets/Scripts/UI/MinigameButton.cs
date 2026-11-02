using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Un boton de la seleccion de minijuegos: carga la escena del juego.
[RequireComponent(typeof(Button))]
public class MinigameButton : MonoBehaviour
{
    [Header("Minijuego")]
    [SerializeField] private string escena;

    private Button boton;

    private void Awake()
    {
        boton = GetComponent<Button>();
        boton.onClick.AddListener(Jugar);
    }

    private void Jugar()
    {
        SceneManager.LoadScene(escena);
    }
}
