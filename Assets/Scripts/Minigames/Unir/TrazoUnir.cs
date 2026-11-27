using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

// ============================================================================
// TrazoUnir.cs  (minijuego "Unir personajes")
// QUE HACE: la linea que se dibuja con el dedo (LineRenderer). Tiene que
//           empezar sobre un personaje, gasta tinta segun lo largo, y avisa si
//           llego al otro personaje o si fallo (choco o se quedo sin tinta).
// DONDE ESTA: en el objeto "Trazo" de la escena Unir.
// PARA CAMBIAR: colores de la linea -> colorNormal / colorFallo.
//               que cuenta como obstaculo -> capaObstaculos.
//               cuanta tinta hay -> el asset de cada nivel ("Tinta").
// AVISA: Unio(desde, hasta) y Fallo.
// ============================================================================

// El trazo con el dedo: arranca sobre un personaje, gasta tinta y avisa si llego al otro o si
// fallo. No sabe de vidas ni de monedas, eso lo decide UnirGame.
[RequireComponent(typeof(LineRenderer))]
public class TrazoUnir : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;
    // Image en modo Filled: el fillAmount es la fraccion de tinta que queda.
    [SerializeField] private Image barraTinta;

    [Header("Umbrales")]
    // Un dedo quieto tiembla y generaria cientos de puntos. En mm para que pida lo mismo en cualquier pantalla.
    [SerializeField] private float distanciaMinimaMm = 1f;
    // [AJUSTE] Capas que cuentan como obstaculo.
    [SerializeField] private LayerMask capaObstaculos;

    [Header("Colores")]
    [SerializeField] private Color colorNormal = Color.black;
    [SerializeField] private Color colorFallo = Color.red;

    // Desde que personaje arranco y a cual llego.
    public event Action<Collider2D, Collider2D> Unio;
    public event Action Fallo;

    public List<Vector3> Puntos => puntos;
    // Fraccion de tinta que queda (1 = llena, 0 = vacia).
    public float TintaRestante => tintaTotal > 0f ? 1f - tintaUsada / tintaTotal : 0f;

    private LineRenderer linea;
    private readonly List<Vector3> puntos = new List<Vector3>();
    private Collider2D personajeA;
    private Collider2D personajeB;
    // Personaje desde el que empezo el trazo.
    private Collider2D inicio;
    private float distanciaMinimaPx;
    private float tintaTotal;
    private float tintaUsada;
    private bool habilitado;
    private int dedoActivo = -1;
    private Vector2 ultimaPantalla;

    private void Awake()
    {
        linea = GetComponent<LineRenderer>();
        if (camara == null) camara = Camera.main;
        linea.useWorldSpace = true;
    }

    private void OnEnable() => EnhancedTouchSupport.Enable();

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
        dedoActivo = -1;
    }

    private void Start()
    {
        distanciaMinimaPx = Pantalla.MmAPixeles(distanciaMinimaMm);
    }

    // Lo llama UnirGame al empezar cada nivel y despues de cada fallo: borra y recarga la tinta.
    public void Habilitar(float tinta, Collider2D a, Collider2D b)
    {
        personajeA = a;
        personajeB = b;
        tintaTotal = tinta;
        Limpiar();
        habilitado = true;
    }

    public void Deshabilitar()
    {
        habilitado = false;
        dedoActivo = -1;
    }

    // Borra la linea y recarga la tinta.
    private void Limpiar()
    {
        puntos.Clear();
        linea.positionCount = 0;
        linea.startColor = colorNormal;
        linea.endColor = colorNormal;
        tintaUsada = 0f;
        dedoActivo = -1;
        inicio = null;
        ActualizarBarra();
    }

    // Sigue al dedo: empezar, seguir o cortar el trazo.
    private void Update()
    {
        if (!habilitado) return;

        foreach (Touch touch in Touch.activeTouches)
        {
            // El trazo dura varios frames: se sigue al dedo por touchId, no por su indice en la lista.
            if (touch.phase == TouchPhase.Began && dedoActivo == -1)
            {
                Empezar(touch);
            }
            else if (touch.touchId == dedoActivo)
            {
                if (touch.phase == TouchPhase.Moved) Seguir(touch);
                // Levantar el dedo antes de llegar no castiga: suele ser un toque sin querer.
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) Limpiar();
            }
            // Seguir puede haber terminado el trazo (llego o fallo): el resto de los dedos no importa.
            if (!habilitado) return;
        }
    }

    // El trazo solo empieza si el dedo cae sobre uno de los dos personajes.
    private void Empezar(Touch touch)
    {
        Vector3 punto = AMundo(touch.screenPosition);
        if (personajeA.OverlapPoint(punto)) inicio = personajeA;
        else if (personajeB.OverlapPoint(punto)) inicio = personajeB;
        else return;

        dedoActivo = touch.touchId;
        ultimaPantalla = touch.screenPosition;
        AgregarPunto(punto);
    }

    // Agrega un punto: gasta tinta, revisa choque con obstaculos y si llego al destino.
    private void Seguir(Touch touch)
    {
        // La distancia minima se mide en pantalla porque es la que recorre el dedo.
        if (Vector2.Distance(ultimaPantalla, touch.screenPosition) < distanciaMinimaPx) return;

        Vector3 punto = AMundo(touch.screenPosition);
        Vector3 anterior = puntos[puntos.Count - 1];

        // Se chequea el segmento entero y no solo el punto nuevo: un dedo rapido salta varios
        // milimetros por frame y atravesaria un obstaculo finito sin pisarlo.
        tintaUsada += Vector3.Distance(anterior, punto);
        bool choco = Physics2D.Linecast(anterior, punto, capaObstaculos).collider != null;

        ultimaPantalla = touch.screenPosition;
        AgregarPunto(punto);

        if (choco || tintaUsada > tintaTotal)
        {
            tintaUsada = Mathf.Min(tintaUsada, tintaTotal);
            ActualizarBarra();
            Fallar();
            return;
        }

        ActualizarBarra();

        Collider2D destino = inicio == personajeA ? personajeB : personajeA;
        if (destino.OverlapPoint(punto))
        {
            habilitado = false;
            dedoActivo = -1;
            Unio?.Invoke(inicio, destino);
        }
    }

    // Pone la linea en rojo y avisa el fallo.
    private void Fallar()
    {
        // Queda en rojo hasta que UnirGame vuelva a habilitar, para que se vea donde fallo.
        habilitado = false;
        dedoActivo = -1;
        linea.startColor = colorFallo;
        linea.endColor = colorFallo;
        Fallo?.Invoke();
    }

    private void AgregarPunto(Vector3 punto)
    {
        puntos.Add(punto);
        linea.positionCount = puntos.Count;
        linea.SetPosition(puntos.Count - 1, punto);
    }

    private Vector3 AMundo(Vector2 pantalla)
    {
        // Camara ortografica: la distancia en Z no cambia nada, solo hay que dejar el punto en el plano 2D.
        Vector3 mundo = camara.ScreenToWorldPoint(pantalla);
        mundo.z = 0f;
        return mundo;
    }

    private void ActualizarBarra()
    {
        if (barraTinta != null) barraTinta.fillAmount = TintaRestante;
    }
}
