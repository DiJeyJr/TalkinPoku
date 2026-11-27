using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

// ============================================================================
// SwipeDetector.cs
// QUE HACE: detecta cuando el jugador desliza el dedo rapido hacia la izquierda
//           o la derecha (swipe) y avisa la direccion.
// DONDE ESTA: en el objeto "Sistemas" de SampleScene. Lo usa RoomNavigator para
//             cambiar de habitacion.
// PARA CAMBIAR: cuanto hay que deslizar -> swipeMinDistanceMm.
//               que tan rapido tiene que ser -> swipeMaxDuration (segundos).
//               sobre que objetos NO cuenta el swipe -> capasQueBloquean.
// AVISA: el evento Swiped(direccion).
// ============================================================================

// Las dos direcciones posibles del swipe.
public enum SwipeDirection
{
    Izquierda,
    Derecha
}

// Detecta swipes horizontales y los avisa por evento. No sabe que hay del otro lado:
// quien quiera reaccionar se suscribe.
public class SwipeDetector : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;

    [Header("Umbrales")]
    // [AJUSTE] Milimetros minimos que tiene que recorrer el dedo de costado.
    // En milimetros: los 150 px de clase equivalen a unos 9 mm en un celular de 400 dpi.
    [SerializeField] private float swipeMinDistanceMm = 9f;
    // [AJUSTE] Segundos maximos del gesto: si tarda mas, es un arrastre y no un swipe.
    [SerializeField] private float swipeMaxDuration = 0.6f;
    // [AJUSTE] Capas de fisica que, si el dedo empieza encima, cancelan el swipe.
    [SerializeField] private LayerMask capasQueBloquean = ~0;

    public event Action<SwipeDirection> Swiped;

    private int dedoActivo = -1;
    private Vector2 posicionInicial;
    private float tiempoInicial;

    private void Awake()
    {
        if (camara == null) camara = Camera.main;
    }

    private void OnEnable() => EnhancedTouchSupport.Enable();

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        // El ScreenStack lo apaga al abrir la tienda con el dedo todavia apoyado. Ese touch termina
        // mientras esta apagado y nunca llega el Ended: si no se limpia, no acepta mas gestos.
        dedoActivo = -1;
    }

    // Anota donde y cuando empieza el dedo; al levantarlo, Evaluar decide si fue un swipe.
    private void Update()
    {
        foreach (Touch touch in Touch.activeTouches)
        {
            // El gesto dura varios frames, asi que el dedo se sigue por touchId y no por indice:
            // activeTouches se compacta cuando se levanta un dedo anterior y los indices se corren.
            if (touch.phase == TouchPhase.Began && dedoActivo == -1)
            {
                // Si el dedo arranca sobre algo que se toca o se arrastra (comida, esponja, cama),
                // el gesto es de ese objeto: arrastrar comida rapido no tiene que cambiar de habitacion.
                if (Physics2D.OverlapPoint(camara.ScreenToWorldPoint(touch.screenPosition), capasQueBloquean) != null) continue;

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

    // Revisa tiempo, direccion y distancia. Si todo da, avisa el swipe.
    private void Evaluar(Vector2 posicionFinal)
    {
        // Un arrastre lento no es un swipe. Sirve para mover objetos, no para cambiar de pantalla.
        if (Time.time - tiempoInicial > swipeMaxDuration) return;

        Vector2 delta = posicionFinal - posicionInicial;

        // Solo importa el eje horizontal: un swipe vertical no tiene que cambiar de habitacion.
        if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y)) return;
        if (Mathf.Abs(delta.x) < Pantalla.MmAPixeles(swipeMinDistanceMm)) return;

        Swiped?.Invoke(delta.x > 0f ? SwipeDirection.Derecha : SwipeDirection.Izquierda);
    }
}
