using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// Permite arrastrar este objeto con el dedo. No sabe que es ni que pasa al soltarlo: avisa por
/// evento y el que lo usa (comida, esponja) decide.
[RequireComponent(typeof(Collider2D))]
public class Draggable : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;

    [Header("Umbrales")]
    [SerializeField] private float moveTolerance = 80f;
    [SerializeField] private bool volverAlOrigen = true;

    public event Action Levantado;
    public event Action<Vector2> Movido;
    public event Action<Vector2> Soltado;

    private Collider2D miCollider;
    private int dedoActivo = -1;
    private Vector2 posicionInicialPantalla;
    private Vector2 offset;
    private bool arrastrando;
    private Vector3 posicionOriginal;

    private void Awake()
    {
        miCollider = GetComponent<Collider2D>();
        if (camara == null) camara = Camera.main;
        posicionOriginal = transform.position;
    }

    private void OnEnable() => EnhancedTouchSupport.Enable();

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        Terminar();
    }

    private void Update()
    {
        foreach (Touch touch in Touch.activeTouches)
        {
            if (touch.phase == TouchPhase.Began && dedoActivo == -1)
            {
                Vector2 mundo = AMundo(touch.screenPosition);
                if (!miCollider.OverlapPoint(mundo)) continue;

                dedoActivo = touch.touchId;
                posicionInicialPantalla = touch.screenPosition;
                // Se guarda donde se agarro el objeto para que no salte a centrarse en el dedo.
                offset = (Vector2)transform.position - mundo;
                arrastrando = false;
                continue;
            }

            if (touch.touchId != dedoActivo) continue;

            if (touch.phase == TouchPhase.Moved)
            {
                // Igual que en clase: un temblor chico al tocar no cuenta como arrastre.
                if (!arrastrando && Vector2.Distance(posicionInicialPantalla, touch.screenPosition) >= moveTolerance)
                {
                    arrastrando = true;
                    Levantado?.Invoke();
                }

                if (arrastrando)
                {
                    Vector2 mundo = AMundo(touch.screenPosition);
                    transform.position = mundo + offset;
                    Movido?.Invoke(mundo);
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (arrastrando) Soltado?.Invoke(AMundo(touch.screenPosition));
                Terminar();
            }
            else if (touch.phase == TouchPhase.Canceled)
            {
                Terminar();
            }
        }
    }

    private void Terminar()
    {
        dedoActivo = -1;
        arrastrando = false;
        if (volverAlOrigen) transform.position = posicionOriginal;
    }

    private Vector2 AMundo(Vector2 pantalla)
    {
        // Camara ortografica: la Z no cambia el resultado en X e Y, asi que no hace falta pasarle
        // la distancia a la camara como en las demos 3D de clase.
        return camara.ScreenToWorldPoint(pantalla);
    }
}
