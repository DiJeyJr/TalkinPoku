using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// La mascota en el minijuego de vuelo: cada tap la hace aletear. Avisa cuando despega, cuando
/// pasa un obstaculo y cuando choca, pero no lleva el puntaje: eso es de VueloGame.
[RequireComponent(typeof(Rigidbody2D))]
public class Volador : MonoBehaviour
{
    [Header("Vuelo")]
    [SerializeField] private float velocidadAleteo = 8f;

    public event Action Despego;
    public event Action PasoObstaculo;
    public event Action Choco;

    private Rigidbody2D body;
    private bool quiereAletear;
    private bool choco;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // Quieta en el aire hasta el primer tap, para que el jugador llegue a leer la indicacion.
        body.simulated = false;
    }

    private void OnEnable() => EnhancedTouchSupport.Enable();

    private void OnDisable() => EnhancedTouchSupport.Disable();

    private void Update()
    {
        if (choco) return;

        // El tap se lee en Update porque en FixedUpdate se pierden los Began que caen entre dos
        // pasos de fisica. Cualquier dedo que apoye aletea, asi que no hace falta seguirlo por touchId.
        foreach (Touch touch in Touch.activeTouches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                quiereAletear = true;
                break;
            }
        }

        if (quiereAletear && !body.simulated)
        {
            body.simulated = true;
            Despego?.Invoke();
        }
    }

    private void FixedUpdate()
    {
        if (!quiereAletear) return;
        quiereAletear = false;

        // Se asigna la velocidad en vez de sumar fuerza: con AddForce dos taps seguidos se acumulan
        // y sale disparada.
        body.linearVelocity = new Vector2(0f, velocidadAleteo);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // El unico trigger de la escena es el hueco entre los tubos.
        if (!choco) PasoObstaculo?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Despues del primer choque sigue cayendo y rebota contra el suelo: eso no es otra derrota.
        if (choco) return;
        choco = true;
        Choco?.Invoke();
    }
}
