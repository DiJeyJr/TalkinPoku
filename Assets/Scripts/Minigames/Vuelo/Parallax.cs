using UnityEngine;

// ============================================================================
// Parallax.cs  (minijuego "Vuelo")
// QUE HACE: mueve el fondo hacia la izquierda para dar sensacion de
//           movimiento. Cada capa va a distinta velocidad (lo lejano mas lento).
// DONDE ESTA: en "Cielo", "Nubes" y "Suelo" de la escena Vuelo.
// PARA CAMBIAR: velocidad de una capa -> factor (1 = igual que los obstaculos;
//               menos de 1 = mas lento, se ve mas lejos).
// ============================================================================

// Corre hacia la izquierda una tira de dibujo que se repite (SpriteRenderer en Draw Mode Tiled).
// Cada capa usa un factor distinto: lo lejano se mueve lento y lo cercano rapido, y esa diferencia
// es la que da sensacion de profundidad.
[RequireComponent(typeof(SpriteRenderer))]
public class Parallax : MonoBehaviour
{
    [Header("Referencias")]
    // De aca sale la velocidad del escenario.
    [SerializeField] private ObstaculoSpawner spawner;

    [Header("Movimiento")]
    // [AJUSTE] Velocidad relativa de esta capa.
    // 1 = a la velocidad de los obstaculos (el piso). Menos de 1 = mas lejos.
    [SerializeField] private float factor = 1f;

    private float anchoTira;
    private float xInicial;

    private void Awake()
    {
        SpriteRenderer dibujo = GetComponent<SpriteRenderer>();
        anchoTira = dibujo.sprite.bounds.size.x * transform.lossyScale.x;
        xInicial = transform.position.x;
    }

    // Cada frame corre la capa a la izquierda y, al pasar una tira entera, la vuelve atras.
    private void Update()
    {
        Vector3 posicion = transform.position;
        posicion.x -= spawner.VelocidadEscenario * factor * Time.deltaTime;
        // Al correrse una tira entera vuelve atras de golpe: como el dibujo se repite, no se nota el salto.
        if (posicion.x <= xInicial - anchoTira) posicion.x += anchoTira;
        transform.position = posicion;
    }
}
