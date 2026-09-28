using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Feedback de un boton: se achica mientras se lo aprieta y hace "pop" cuando se lo toca.
[RequireComponent(typeof(Button))]
public class BotonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sonidos")]
    [SerializeField] private AudioClip sonido;

    [Header("Feedback")]
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

    public void OnPointerDown(PointerEventData evento)
    {
        if (!boton.IsInteractable()) return;
        // La escala de ahora y no la de Awake: el indicador de habitacion agranda el icono actual.
        escalaSuelto = transform.localScale;
        transform.localScale = escalaSuelto * escalaApretado;
        apretado = true;
    }

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
