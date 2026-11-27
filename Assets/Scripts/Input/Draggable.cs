using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

// ============================================================================
// Draggable.cs
// QUE HACE: permite agarrar este objeto con el dedo y arrastrarlo. Solo mueve el
//           objeto y avisa por eventos; no sabe que pasa al soltarlo.
// DONDE ESTA: en la comida, la esponja, los ingredientes de Cocina, las basuras
//             de Limpieza, etc. Necesita un Collider2D en el mismo objeto.
// PARA CAMBIAR: cuanto hay que mover el dedo para que cuente como arrastre ->
//               moveToleranceMm. Si vuelve a su lugar al soltarlo -> volverAlOrigen.
// AVISA: Levantado (empezo a arrastrar), Movido(posicion) y Soltado(posicion).
//        El que lo usa (por ejemplo FoodItem o Sponge) se suscribe y decide.
// ============================================================================

// Permite arrastrar este objeto con el dedo. No sabe que es ni que pasa al soltarlo: avisa por
// evento y el que lo usa (comida, esponja) decide.
[RequireComponent(typeof(Collider2D))]
public class Draggable : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;

    [Header("Umbrales")]
    // [AJUSTE] Milimetros que tiene que moverse el dedo para que empiece el arrastre.
    // En milimetros, igual que en TouchTarget.
    [SerializeField] private float moveToleranceMm = 5f;
    // [AJUSTE] true = al soltarlo vuelve a donde estaba al principio.
    [SerializeField] private bool volverAlOrigen = true;

    // Eventos que dispara este script.
    public event Action Levantado;
    public event Action<Vector2> Movido;
    public event Action<Vector2> Soltado;

    public bool Arrastrando => arrastrando;

    // Para lo que se crea por codigo (las basuras de Limpieza), que no pasa por el Inspector.
    public bool VolverAlOrigen
    {
        get => volverAlOrigen;
        set => volverAlOrigen = value;
    }

    private Collider2D miCollider;
    // Id del dedo que lo esta agarrando (-1 = ninguno).
    private int dedoActivo = -1;
    private Vector2 posicionInicialPantalla;
    // Distancia entre el dedo y el centro del objeto, para que no salte.
    private Vector2 offset;
    private bool arrastrando;
    private Vector3 posicionOriginal;

    private void Awake()
    {
        miCollider = GetComponent<Collider2D>();
        if (camara == null) camara = Camera.main;
        posicionOriginal = transform.position;
    }

    // EnhancedTouch es la API de toques del Input System nuevo: hay que prenderla para leer los dedos.
    private void OnEnable() => EnhancedTouchSupport.Enable();

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        Terminar();
    }

    // Cada frame revisa todos los dedos apoyados y sigue al que agarro este objeto.
    private void Update()
    {
        foreach (Touch touch in Touch.activeTouches)
        {
            // 1) Un dedo nuevo: si cayo sobre este objeto, lo agarra.
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

            // Los demas dedos no le importan.
            if (touch.touchId != dedoActivo) continue;

            // 2) El dedo se movio: si paso la tolerancia empieza el arrastre y el objeto lo sigue.
            if (touch.phase == TouchPhase.Moved)
            {
                // Igual que en clase: un temblor chico al tocar no cuenta como arrastre.
                if (!arrastrando && Vector2.Distance(posicionInicialPantalla, touch.screenPosition) >= Pantalla.MmAPixeles(moveToleranceMm))
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
            // 3) Se levanto el dedo: avisa donde lo solto.
            else if (touch.phase == TouchPhase.Ended)
            {
                if (arrastrando) Soltado?.Invoke(AMundo(touch.screenPosition));
                Terminar();
            }
            // 4) El sistema cancelo el toque (llamada, gesto del sistema): se suelta sin avisar.
            else if (touch.phase == TouchPhase.Canceled)
            {
                Terminar();
            }
        }
    }

    // Deja todo listo para el proximo toque y, si corresponde, vuelve el objeto a su lugar.
    private void Terminar()
    {
        dedoActivo = -1;
        arrastrando = false;
        if (volverAlOrigen) transform.position = posicionOriginal;
    }

    // Pasa de pixeles de pantalla a coordenadas del mundo.
    private Vector2 AMundo(Vector2 pantalla)
    {
        // Camara ortografica: la Z no cambia el resultado en X e Y, asi que no hace falta pasarle
        // la distancia a la camara como en las demos 3D de clase.
        return camara.ScreenToWorldPoint(pantalla);
    }
}
