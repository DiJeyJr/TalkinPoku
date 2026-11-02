using UnityEngine;

/// Lanza obstaculos desde la derecha cada cierto tiempo. Los crea todos al arrancar y los recicla,
/// porque instanciar y destruir uno por tubo genera basura y el GC da tirones en el celular.
public class ObstaculoSpawner : MonoBehaviour
{
    [Header("Referencias")]
    // Obstaculo apagado en la escena, con Rigidbody2D Kinematic: se mueve por velocidad y el
    // choque con la mascota no lo empuja.
    [SerializeField] private Rigidbody2D modelo;
    [SerializeField] private int cantidad = 5;

    [Header("Movimiento")]
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float intervalo = 1.8f;
    [SerializeField] private float xAparicion = 7f;
    [SerializeField] private float xReciclado = -7f;

    [Header("Hueco")]
    [SerializeField] private float alturaMinima = -3f;
    [SerializeField] private float alturaMaxima = 4f;

    [Header("Dificultad")]
    // Velocidad e intervalo de arriba son los del primer obstaculo; con el puntaje se acercan a estos
    // dos topes. Un intervalo minimo muy bajo amontona obstaculos: no bajar de 1,2 sin probar en el celular.
    [SerializeField] private float velocidadMaxima = 5.5f;
    [SerializeField] private float intervaloMinimo = 1.4f;
    // Puntaje en el que se llega al tope.
    [SerializeField] private int puntosParaMaximo = 30;
    // Eje X: avance (0 a 1) hasta el tope; eje Y: cuanto de la distancia al tope ya se recorrio.
    [SerializeField] private AnimationCurve curvaDificultad = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private Rigidbody2D[] obstaculos;
    private int siguiente;
    private float proximoLanzamiento;
    private bool corriendo;
    private float velocidadActual;
    private float intervaloActual;

    // A esta velocidad se corre el escenario: el piso va igual que los obstaculos y el fondo mas lento.
    // Antes de arrancar y despues de chocar es 0, asi el escenario se queda quieto.
    public float VelocidadEscenario => corriendo ? velocidadActual : 0f;

    private void Awake()
    {
        velocidadActual = velocidad;
        intervaloActual = intervalo;
        obstaculos = new Rigidbody2D[cantidad];
        for (int i = 0; i < cantidad; i++)
        {
            obstaculos[i] = Instantiate(modelo, transform);
            obstaculos[i].gameObject.SetActive(false);
        }
    }

    public void Arrancar()
    {
        ActualizarDificultad(0);
        corriendo = true;
        proximoLanzamiento = Time.time;
    }

    // Solo afecta a los obstaculos que se lancen de ahi en adelante: los que ya vuelan no cambian de velocidad.
    public void ActualizarDificultad(int puntos)
    {
        float avance = puntosParaMaximo > 0 ? Mathf.Clamp01((float)puntos / puntosParaMaximo) : 1f;
        float k = Mathf.Clamp01(curvaDificultad.Evaluate(avance));
        velocidadActual = Mathf.Lerp(velocidad, velocidadMaxima, k);
        intervaloActual = Mathf.Lerp(intervalo, intervaloMinimo, k);
    }

    public void Frenar()
    {
        corriendo = false;
        foreach (Rigidbody2D obstaculo in obstaculos)
        {
            obstaculo.linearVelocity = Vector2.zero;
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

        foreach (Rigidbody2D obstaculo in obstaculos)
        {
            if (obstaculo.gameObject.activeSelf && obstaculo.position.x < xReciclado)
            {
                obstaculo.gameObject.SetActive(false);
            }
        }
    }

    private void Lanzar()
    {
        // Se usan en ronda. Con 5 alcanza: a 3 u/s cada uno tarda unos 4,7 s en cruzar y sale uno
        // cada 1,8 s, asi que nunca hay mas de 3 en pantalla; en el tope (5,5 u/s cada 1,4 s) son 2.
        // Si se cambian esos valores, revisar.
        Rigidbody2D obstaculo = obstaculos[siguiente];
        siguiente = (siguiente + 1) % obstaculos.Length;

        // Se mueve el transform antes de prenderlo: con el objeto apagado el Rigidbody2D no simula.
        obstaculo.transform.position = new Vector3(xAparicion, Random.Range(alturaMinima, alturaMaxima), 0f);
        obstaculo.gameObject.SetActive(true);
        obstaculo.linearVelocity = new Vector2(-velocidadActual, 0f);
    }
}
