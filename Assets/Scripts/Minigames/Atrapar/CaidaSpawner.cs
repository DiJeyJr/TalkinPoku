using System;
using UnityEngine;

/// Tira ingredientes y objetos malos desde arriba. Los crea todos al arrancar y los recicla, igual
/// que ObstaculoSpawner en el vuelo, para no generar basura con un Instantiate por caida.
public class CaidaSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;
    // Objeto apagado en la escena, con Rigidbody2D Dynamic sin gravedad y collider trigger.
    [SerializeField] private ObjetoQueCae modelo;
    [SerializeField] private int cantidad = 10;
    [SerializeField] private Sprite[] spritesBuenos;
    [SerializeField] private Sprite[] spritesMalos;

    [Header("Caida")]
    [SerializeField] private float yAparicion = 6f;
    [SerializeField] private float yReciclado = -6f;
    // Distancia al borde de la pantalla, para que no aparezcan cortados por la mitad.
    [SerializeField] private float margenLateral = 0.5f;
    // Grados por segundo, al azar para cada lado. Es solo para que se vea mas vivo.
    [SerializeField] private float giroMaximo = 90f;

    [Header("Dificultad")]
    // Los primeros valores son los del arranque; con el puntaje se acercan a los maximos.
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float velocidadMaxima = 7f;
    [SerializeField] private float intervalo = 1.2f;
    [SerializeField] private float intervaloMinimo = 0.5f;
    [SerializeField, Range(0f, 1f)] private float proporcionMalos = 0.2f;
    [SerializeField, Range(0f, 1f)] private float proporcionMalosMaxima = 0.45f;
    // Puntaje en el que se llega al tope.
    [SerializeField] private int puntosParaMaximo = 40;
    // Eje X: avance (0 a 1) hasta el tope; eje Y: cuanto de la distancia al tope ya se recorrio.
    [SerializeField] private AnimationCurve curvaDificultad = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    // Un bueno que llego al piso sin que lo atrapen. Los malos que caen no le importan a nadie.
    public event Action SeCayoBueno;

    private ObjetoQueCae[] objetos;
    private float xMaxima;
    private float proximoLanzamiento;
    private bool corriendo;
    private float velocidadActual;
    private float intervaloActual;
    private float proporcionMalosActual;

    private void Awake()
    {
        if (camara == null) camara = Camera.main;
        objetos = new ObjetoQueCae[cantidad];
        for (int i = 0; i < cantidad; i++)
        {
            objetos[i] = Instantiate(modelo, transform);
            objetos[i].gameObject.SetActive(false);
        }
        ActualizarDificultad(0);
    }

    private void Start()
    {
        // Con el ancho real de la camara, asi en un celular mas angosto no caen fuera de la pantalla.
        xMaxima = camara.ViewportToWorldPoint(Vector3.one).x - margenLateral;
    }

    public void Arrancar()
    {
        corriendo = true;
        proximoLanzamiento = Time.time;
    }

    // Solo cambia los que se tiren de ahi en adelante: los que ya estan cayendo siguen igual.
    public void ActualizarDificultad(int puntos)
    {
        float avance = puntosParaMaximo > 0 ? Mathf.Clamp01((float)puntos / puntosParaMaximo) : 1f;
        float k = Mathf.Clamp01(curvaDificultad.Evaluate(avance));
        velocidadActual = Mathf.Lerp(velocidad, velocidadMaxima, k);
        intervaloActual = Mathf.Lerp(intervalo, intervaloMinimo, k);
        proporcionMalosActual = Mathf.Lerp(proporcionMalos, proporcionMalosMaxima, k);
    }

    public void Frenar()
    {
        corriendo = false;
        foreach (ObjetoQueCae objeto in objetos)
        {
            objeto.Frenar();
        }
    }

    private void Update()
    {
        if (!corriendo) return;

        if (Time.time >= proximoLanzamiento)
        {
            Lanzar();
            proximoLanzamiento = Time.time + intervaloActual;
        }

        foreach (ObjetoQueCae objeto in objetos)
        {
            if (objeto.gameObject.activeSelf && objeto.Posicion.y < yReciclado)
            {
                objeto.gameObject.SetActive(false);
                if (objeto.EsBueno) SeCayoBueno?.Invoke();
            }
        }
    }

    private void Lanzar()
    {
        // El primero libre y no en ronda: en el tope sale uno cada 0,5 s y tarda casi 2 s en caer,
        // asi que en ronda podria reciclar uno que todavia esta en pantalla. Si no hay libre, se saltea.
        ObjetoQueCae libre = null;
        foreach (ObjetoQueCae objeto in objetos)
        {
            if (!objeto.gameObject.activeSelf)
            {
                libre = objeto;
                break;
            }
        }
        if (libre == null) return;

        bool esBueno = UnityEngine.Random.value >= proporcionMalosActual;
        Sprite[] sprites = esBueno ? spritesBuenos : spritesMalos;
        Sprite sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];

        Vector3 posicion = new Vector3(UnityEngine.Random.Range(-xMaxima, xMaxima), yAparicion, 0f);
        libre.Caer(posicion, sprite, esBueno, velocidadActual, UnityEngine.Random.Range(-giroMaximo, giroMaximo));
    }
}
