using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// Detecta tap y pulsacion prolongada sobre este objeto, con el mismo criterio que el detector
/// de la clase 2. Avisa por evento; que significa cada gesto lo decide otro script.
[RequireComponent(typeof(Collider2D))]
public class TouchTarget : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;

    [Header("Umbrales")]
    [SerializeField] private float tapMaxDuration = 0.35f;
    [SerializeField] private float holdMinDuration = 0.8f;
    [SerializeField] private float moveTolerance = 80f;

    public event Action Tocado;
    public event Action HoldEmpezado;
    public event Action HoldTerminado;

    private Collider2D miCollider;
    private int dedoActivo = -1;
    private Vector2 posicionInicial;
    private float tiempoInicial;
    private bool holdDetectado;

    public bool Sosteniendo => holdDetectado;

    private void Awake()
    {
        miCollider = GetComponent<Collider2D>();
        if (camara == null) camara = Camera.main;
    }

    private void OnEnable() => EnhancedTouchSupport.Enable();

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        Limpiar();
    }

    private void Update()
    {
        foreach (Touch touch in Touch.activeTouches)
        {
            if (touch.phase == TouchPhase.Began && dedoActivo == -1)
            {
                if (!miCollider.OverlapPoint(camara.ScreenToWorldPoint(touch.screenPosition))) continue;

                dedoActivo = touch.touchId;
                posicionInicial = touch.screenPosition;
                tiempoInicial = Time.time;
                holdDetectado = false;
                continue;
            }

            if (touch.touchId != dedoActivo) continue;

            float duracion = Time.time - tiempoInicial;
            float distancia = Vector2.Distance(posicionInicial, touch.screenPosition);

            // En clase el hold se mira solo en Stationary. Aca tambien en Moved, porque en un
            // celular real el dedo apoyado tiembla y a veces no llega a quedar Stationary.
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                if (!holdDetectado && duracion >= holdMinDuration && distancia <= moveTolerance)
                {
                    holdDetectado = true;
                    HoldEmpezado?.Invoke();
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (holdDetectado) HoldTerminado?.Invoke();
                else if (duracion <= tapMaxDuration && distancia <= moveTolerance) Tocado?.Invoke();
                Limpiar();
            }
            else if (touch.phase == TouchPhase.Canceled)
            {
                if (holdDetectado) HoldTerminado?.Invoke();
                Limpiar();
            }
        }
    }

    private void Limpiar()
    {
        dedoActivo = -1;
        holdDetectado = false;
    }
}
