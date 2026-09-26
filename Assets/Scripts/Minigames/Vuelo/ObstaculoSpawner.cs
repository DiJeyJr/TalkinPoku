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

    private Rigidbody2D[] obstaculos;
    private int siguiente;
    private float proximoLanzamiento;
    private bool corriendo;

    private void Awake()
    {
        obstaculos = new Rigidbody2D[cantidad];
        for (int i = 0; i < cantidad; i++)
        {
            obstaculos[i] = Instantiate(modelo, transform);
            obstaculos[i].gameObject.SetActive(false);
        }
    }

    public void Arrancar()
    {
        corriendo = true;
        proximoLanzamiento = Time.time;
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
            proximoLanzamiento = Time.time + intervalo;
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
        // cada 1,8 s, asi que nunca hay mas de 3 en pantalla. Si se cambian esos valores, revisar.
        Rigidbody2D obstaculo = obstaculos[siguiente];
        siguiente = (siguiente + 1) % obstaculos.Length;

        // Se mueve el transform antes de prenderlo: con el objeto apagado el Rigidbody2D no simula.
        obstaculo.transform.position = new Vector3(xAparicion, Random.Range(alturaMinima, alturaMaxima), 0f);
        obstaculo.gameObject.SetActive(true);
        obstaculo.linearVelocity = new Vector2(-velocidad, 0f);
    }
}
