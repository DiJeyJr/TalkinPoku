using System.Collections;
using UnityEngine;

// ============================================================================
// LoadingScreen.cs
// QUE HACE: la pantalla de carga al abrir el juego. Se queda un momento y
//           despues se desvanece.
// DONDE ESTA: en "Canvas/PantallaCarga" de SampleScene.
// PARA CAMBIAR: cuanto se ve -> segundosVisible. Cuanto tarda en desvanecerse
//               -> segundosFundido.
// ============================================================================

// La pantalla de carga de Martina al abrir el juego: queda un momento y se desvanece. Tapa todo
// mientras se restaura el guardado y se arma la casa, que es lo que tarda en el primer cuadro.
[RequireComponent(typeof(CanvasGroup))]
public class LoadingScreen : MonoBehaviour
{
    [Header("Tiempos")]
    // [AJUSTE] Segundos visible y segundos de desvanecido.
    [SerializeField] private float segundosVisible = 1.5f;
    [SerializeField] private float segundosFundido = 0.4f;

    // Para lo que no tiene que pasar detras de la pantalla de carga, como las monedas que vuelan al
    // volver de un minijuego. Se prende en Awake porque los Start de los demas pueden correr antes.
    public static bool Tapando { get; private set; }

    private void Awake() => Tapando = true;

    private void OnDestroy() => Tapando = false;

    // Corrutina: espera, baja la transparencia de a poco y se apaga.
    private IEnumerator Start()
    {
        CanvasGroup grupo = GetComponent<CanvasGroup>();
        grupo.alpha = 1f;
        grupo.blocksRaycasts = true;
        yield return new WaitForSecondsRealtime(segundosVisible);

        for (float t = 0f; t < segundosFundido; t += Time.unscaledDeltaTime)
        {
            grupo.alpha = 1f - t / segundosFundido;
            yield return null;
        }
        Tapando = false;
        gameObject.SetActive(false);
    }
}
