using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public enum SwipeDirection
{
    Izquierda,
    Derecha
}

/// Detecta swipes horizontales y los avisa por evento. No sabe que hay del otro lado:
/// quien quiera reaccionar se suscribe.
public class SwipeDetector : MonoBehaviour
{
    [Header("Umbrales")]
    [SerializeField] private float swipeMinDistance = 150f;
    [SerializeField] private float swipeMaxDuration = 0.6f;

    public event Action<SwipeDirection> Swiped;

    private int dedoActivo = -1;
    private Vector2 posicionInicial;
    private float tiempoInicial;

    private void OnEnable() => EnhancedTouchSupport.Enable();

    private void OnDisable() => EnhancedTouchSupport.Disable();

    private void Update()
    {
        foreach (Touch touch in Touch.activeTouches)
        {
            // El gesto dura varios frames, asi que el dedo se sigue por touchId y no por indice:
            // activeTouches se compacta cuando se levanta un dedo anterior y los indices se corren.
            if (touch.phase == TouchPhase.Began && dedoActivo == -1)
            {
                dedoActivo = touch.touchId;
                posicionInicial = touch.screenPosition;
                tiempoInicial = Time.time;
                continue;
            }

            if (touch.touchId != dedoActivo) continue;

            if (touch.phase == TouchPhase.Ended)
            {
                Evaluar(touch.screenPosition);
                dedoActivo = -1;
            }
            else if (touch.phase == TouchPhase.Canceled)
            {
                // Una llamada entrante o el gesto del sistema cancelan el touch. Si no se limpia
                // el estado, el proximo gesto arranca con la posicion vieja.
                dedoActivo = -1;
            }
        }
    }

    private void Evaluar(Vector2 posicionFinal)
    {
        // Un arrastre lento no es un swipe. Sirve para mover objetos, no para cambiar de pantalla.
        if (Time.time - tiempoInicial > swipeMaxDuration) return;

        Vector2 delta = posicionFinal - posicionInicial;

        // Solo importa el eje horizontal: un swipe vertical no tiene que cambiar de habitacion.
        if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y)) return;
        if (Mathf.Abs(delta.x) < swipeMinDistance) return;

        Swiped?.Invoke(delta.x > 0f ? SwipeDirection.Derecha : SwipeDirection.Izquierda);
    }
}
