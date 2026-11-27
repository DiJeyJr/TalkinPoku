using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ============================================================================
// BotonFeedback.cs
// QUE HACE: hace que un boton se achique mientras se lo aprieta y suene al
//           tocarlo.
// DONDE ESTA: en casi todos los botones de la UI (salir, reintentar, etc).
// PARA CAMBIAR: cuanto se achica -> escalaApretado. El sonido -> "Sonido".
// ============================================================================

// Feedback de un boton: se achica mientras se lo aprieta y hace "pop" cuando se lo toca.
[RequireComponent(typeof(Button))]
public class BotonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonido;

    [Header("Feedback")]
    // [AJUSTE] Escala mientras esta apretado (0.92 = 8% mas chico).
    [SerializeField] private float escalaApretado = 0.92f;

    private Button boton;
    private Vector3 escalaSuelto;
    private bool apretado;

    private void Awake()
    {
        boton = GetComponent<Button>();
        // Con onClick y no al apoyar: suena solo si el toque cuenta (boton habilitado y se solto encima).
        boton.onClick.AddListener(Sonar);
    }

    private void OnDisable()
    {
        // Si la pantalla se cierra con el dedo apoyado, el boton no puede quedar achicado.
        Soltar();
    }

    // Unity lo llama al apoyar el dedo: achica el boton.
    public void OnPointerDown(PointerEventData evento)
    {
        if (!boton.IsInteractable()) return;
        // La escala de ahora y no la de Awake: el indicador de habitacion agranda el icono actual.
        escalaSuelto = transform.localScale;
        transform.localScale = escalaSuelto * escalaApretado;
        apretado = true;
    }

    // Unity lo llama al levantar el dedo: vuelve al tamano normal.
    public void OnPointerUp(PointerEventData evento)
    {
        Soltar();
    }

    private void Soltar()
    {
        if (!apretado) return;
        apretado = false;
        transform.localScale = escalaSuelto;
    }

    private void Sonar()
    {
        SoundPlayer.Reproducir(sonido);
    }
}
