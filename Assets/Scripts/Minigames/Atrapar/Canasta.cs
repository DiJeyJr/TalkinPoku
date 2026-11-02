using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// La canasta de abajo. Se mueve inclinando el celular o arrastrando el dedo, y avisa que atrapo,
/// pero no lleva puntos ni vidas: eso es de AtraparGame.
[RequireComponent(typeof(Rigidbody2D))]
public class Canasta : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;

    [Header("Inclinacion")]
    // Si se apaga, solo se juega con el dedo (para probar o si el sensor anda mal).
    [SerializeField] private bool usarInclinacion = true;
    // Unidades de mundo por cada g de inclinacion hacia el costado.
    [SerializeField] private float tiltSensitivity = 8f;

    [Header("Suavizado")]
    // El de la clase 5: mas alto responde mas rapido pero tiembla mas.
    [SerializeField] private float smoothing = 5f;
    [SerializeField] private float followSpeed = 12f;

    [Header("Limites")]
    // Media canasta, para que no se salga de la pantalla.
    [SerializeField] private float margenLateral = 0.8f;

    public event Action<ObjetoQueCae> Atrapo;

    private Rigidbody2D body;
    private float xMaxima;
    private float xObjetivo;
    private int dedoActivo = -1;
    private float offsetDedo;

    private Vector3 rawAcceleration;
    private Vector3 smoothedAcceleration;
    private Vector3 baselineAcceleration;
    private bool calibrarEnLaProximaLectura = true;

    private bool InclinacionDisponible => usarInclinacion && Accelerometer.current != null;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        if (camara == null) camara = Camera.main;
        xObjetivo = body.position.x;
    }

    private void Start()
    {
        xMaxima = camara.ViewportToWorldPoint(Vector3.one).x - margenLateral;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        // Los sensores vienen apagados en el Input System: sin esto ReadValue da siempre cero.
        if (Accelerometer.current != null) InputSystem.EnableDevice(Accelerometer.current);
        calibrarEnLaProximaLectura = true;
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        // Sin contador: ShakeDetector es el otro que lo usa y nunca estan en la misma escena.
        if (Accelerometer.current != null) InputSystem.DisableDevice(Accelerometer.current);
        dedoActivo = -1;
    }

    // Para el boton "Calibrar": el reposo pasa a ser como el jugador tiene el celular ahora, no el
    // celular plano sobre la mesa. No se toma aca mismo porque el sensor puede no haber leido todavia.
    public void Calibrar()
    {
        calibrarEnLaProximaLectura = true;
    }

    private void Update()
    {
        LeerDedo();

        // El dedo manda sobre la inclinacion: si esta apoyado, el sensor no mueve nada.
        if (dedoActivo == -1 && InclinacionDisponible) LeerInclinacion();
    }

    private void LeerDedo()
    {
        foreach (Touch touch in Touch.activeTouches)
        {
            // Sirve cualquier lugar de la pantalla, no hace falta tocar la canasta: con el dedo
            // encima se taparian justo los objetos que estan por caer en ella.
            if (touch.phase == TouchPhase.Began && dedoActivo == -1)
            {
                dedoActivo = touch.touchId;
                offsetDedo = xObjetivo - AMundoX(touch.screenPosition);
                continue;
            }

            if (touch.touchId != dedoActivo) continue;

            if (touch.phase == TouchPhase.Moved)
            {
                // Con offset para que la canasta no salte a ponerse debajo del dedo.
                xObjetivo = Mathf.Clamp(AMundoX(touch.screenPosition) + offsetDedo, -xMaxima, xMaxima);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                dedoActivo = -1;
            }
        }
    }

    private void LeerInclinacion()
    {
        rawAcceleration = Accelerometer.current.acceleration.ReadValue();

        if (calibrarEnLaProximaLectura)
        {
            // Arranca el suavizado desde la lectura real: desde cero tardaria en llegar y la canasta
            // se iria a un costado al empezar.
            smoothedAcceleration = rawAcceleration;
            baselineAcceleration = rawAcceleration;
            calibrarEnLaProximaLectura = false;
        }

        // Aca si se suaviza, al reves que en el shake: es un movimiento continuo y el dato crudo
        // tiembla. El Lerp funciona como un filtro pasa bajos.
        smoothedAcceleration = Vector3.Lerp(smoothedAcceleration, rawAcceleration, smoothing * Time.deltaTime);

        Vector3 relativa = smoothedAcceleration - baselineAcceleration;
        float destino = Mathf.Clamp(relativa.x * tiltSensitivity, -xMaxima, xMaxima);
        xObjetivo = Mathf.Lerp(xObjetivo, destino, followSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // MovePosition en vez de mover el transform: el Rigidbody2D Kinematic llega con la fisica y
        // los triggers no se pierden.
        body.MovePosition(new Vector2(xObjetivo, body.position.y));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Lo unico que entra en la canasta son objetos que caen; igual se chequea por si acaso.
        if (!other.TryGetComponent(out ObjetoQueCae objeto)) return;

        objeto.gameObject.SetActive(false);
        Atrapo?.Invoke(objeto);
    }

    private float AMundoX(Vector2 pantalla)
    {
        // Camara ortografica: la Z no cambia X e Y, no hace falta la distancia de las demos 3D.
        return camara.ScreenToWorldPoint(pantalla).x;
    }
}
