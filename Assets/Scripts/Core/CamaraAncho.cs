using UnityEngine;
using UnityEngine.SceneManagement;

// ============================================================================
// CamaraAncho.cs
// QUE HACE: ajusta el zoom de la camara para que siempre entre todo el ancho
//           del arte (1080 px), en cualquier forma de celular.
// DONDE ESTA: se agrega solo a la Main Camera de cada escena al cargarla
//             (ver AlCargarEscena), no hace falta ponerlo a mano.
// PARA CAMBIAR: cuanto se ve de alto o de ancho -> tamanoMinimo y anchoVisible.
// ============================================================================

// El arte esta pensado para 1080 px de ancho. En un celular mas alargado que 16:9 se ve mas alto en
// vez de recortar los costados.
[DefaultExecutionOrder(-100)]   // corre antes que los demas scripts, que leen los bordes de la camara
[RequireComponent(typeof(Camera))]
public class CamaraAncho : MonoBehaviour
{
    [Header("Medidas")]
    // [AJUSTE] Mitad del alto visible: 1920 px a 100 px por unidad.
    [SerializeField] private float tamanoMinimo = 9.6f;
    // [AJUSTE] Ancho que siempre tiene que entrar: 1080 px a 100 px por unidad.
    [SerializeField] private float anchoVisible = 10.8f;

    // Al arrancar calcula el zoom (orthographicSize) segun la forma de la pantalla.
    private void Awake()
    {
        Camera camara = GetComponent<Camera>();
        // orthographicSize es la mitad del alto y el ancho visible es alto * aspect.
        camara.orthographicSize = Mathf.Max(tamanoMinimo, anchoVisible / camara.aspect / 2f);
    }

    // Asi no hay que agregarlo a mano en cada escena. sceneLoaded llega despues de los Awake y antes
    // de los Start, que es donde ScreenBounds, la canasta y el spawner leen los bordes.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Registrar()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    // Cada vez que carga una escena, le pone este componente a la camara principal si no lo tiene.
    private static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        Camera camara = Camera.main;
        if (camara == null || !camara.orthographic) return;
        if (camara.GetComponent<CamaraAncho>() == null) camara.gameObject.AddComponent<CamaraAncho>();
    }
}
