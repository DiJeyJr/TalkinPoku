using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

// ============================================================================
// TouchTarget.cs
// QUE HACE: detecta dos gestos sobre este objeto: tap (toque corto) y hold
//           (mantener apretado). Solo avisa; no decide que pasa.
// DONDE ESTA: en la Cama del dormitorio, las flechas de la Mesada, la Olla de
//             Cocina, las basuras, las celdas de Camino, etc. Necesita Collider2D.
// PARA CAMBIAR: duracion maxima de un tap -> tapMaxDuration.
//               cuanto hay que mantener para el hold -> holdMinDuration.
//               cuanto se puede mover el dedo sin cancelar -> moveToleranceMm.
// AVISA: Tocado (tap) y HoldEmpezado (empezo a mantener apretado).
// ============================================================================

// Detecta tap y pulsacion prolongada sobre este objeto, con el mismo criterio que el detector
// de la clase 2. Avisa por evento; que significa cada gesto lo decide otro script.
[RequireComponent(typeof(Collider2D))]
public class TouchTarget : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;

    [Header("Umbrales")]
    // [AJUSTE] Segundos maximos para que cuente como tap.
    [SerializeField] private float tapMaxDuration = 0.35f;
    // [AJUSTE] Segundos que hay que mantener apretado para el hold.
    [SerializeField] private float holdMinDuration = 0.8f;
    // [AJUSTE] Milimetros que se puede mover el dedo y seguir contando como tap/hold.
    // En milimetros: los 80 px de clase equivalen a unos 5 mm en un celular de 400 dpi.
    [SerializeField] private float moveToleranceMm = 5f;

    public event Action Tocado;
    public event Action HoldEmpezado;

    private Collider2D miCollider;
    private int dedoActivo = -1;
    private Vector2 posicionInicial;
    private float tiempoInicial;
    private bool holdDetectado;

    // true mientras el dedo sigue apretando despues de detectar el hold.
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

    // Sigue al dedo que empezo sobre el objeto y mide cuanto tiempo y cuanto se movio.
    private void Update()
    {
        foreach (Touch touch in Touch.activeTouches)
        {
            // Empieza un toque: solo cuenta si cae sobre el collider de este objeto.
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
            float moveTolerance = Pantalla.MmAPixeles(moveToleranceMm);

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
            // Al levantar el dedo: si fue corto y casi sin moverse, es un tap.
            else if (touch.phase == TouchPhase.Ended)
            {
                if (!holdDetectado && duracion <= tapMaxDuration && distancia <= moveTolerance) Tocado?.Invoke();
                Limpiar();
            }
            else if (touch.phase == TouchPhase.Canceled)
            {
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
