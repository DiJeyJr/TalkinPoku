using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ============================================================================
// MinigameButton.cs
// QUE HACE: un boton de la pantalla de minijuegos. Al tocarlo carga la escena
//           de ese minijuego.
// DONDE ESTA: en cada boton de "Canvas/Pantallas/Minijuegos/Contenido/Grilla".
// PARA CAMBIAR: que escena abre -> "Escena" (nombre exacto de la escena; tiene
//               que estar agregada en File > Build Profiles / Build Settings).
// ============================================================================

// Un boton de la seleccion de minijuegos: carga la escena del juego.
[RequireComponent(typeof(Button))]
public class MinigameButton : MonoBehaviour
{
    [Header("Minijuego")]
    // [AJUSTE] Nombre de la escena a cargar.
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
