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

    [Header("Inclinacion")]
    // Solo el dibujo: el Rigidbody2D tiene la rotacion congelada para que los choques no lo hagan girar.
    [SerializeField] private Transform dibujo;
    // Grados por cada unidad por segundo de velocidad vertical, con tope para arriba y para abajo.
    [SerializeField] private float gradosPorVelocidad = 4f;
    [SerializeField] private float inclinacionMaxima = 30f;
    [SerializeField] private float inclinacionMinima = -60f;

    [Header("Suavizado")]
    [SerializeField] private float suavizado = 10f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoAleteo;
    [SerializeField] private AudioClip sonidoChoque;

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
        // Antes del return: despues del choque sigue cayendo, y se ve mejor si cae de nariz.
        Inclinar();

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

    private void Inclinar()
    {
        // Nariz para arriba al subir y para abajo al caer. Va con el suavizado de la clase 5 (Lerp con
        // smoothing * deltaTime), asi el cambio de golpe del aleteo no hace saltar el dibujo.
        float objetivo = Mathf.Clamp(body.linearVelocity.y * gradosPorVelocidad, inclinacionMinima, inclinacionMaxima);
        float angulo = Mathf.LerpAngle(dibujo.localEulerAngles.z, objetivo, suavizado * Time.deltaTime);
        dibujo.localRotation = Quaternion.Euler(0f, 0f, angulo);
    }

    private void FixedUpdate()
    {
        if (!quiereAletear) return;
        quiereAletear = false;

        // Se asigna la velocidad en vez de sumar fuerza: con AddForce dos taps seguidos se acumulan
        // y sale disparada.
        body.linearVelocity = new Vector2(0f, velocidadAleteo);
        SoundPlayer.Reproducir(sonidoAleteo);
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
        SoundPlayer.Reproducir(sonidoChoque);
        Choco?.Invoke();
    }
}
