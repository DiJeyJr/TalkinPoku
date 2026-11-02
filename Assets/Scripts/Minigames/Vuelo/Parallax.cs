using UnityEngine;

// Corre hacia la izquierda una tira de dibujo que se repite (SpriteRenderer en Draw Mode Tiled).
// Cada capa usa un factor distinto: lo lejano se mueve lento y lo cercano rapido, y esa diferencia
// es la que da sensacion de profundidad.
[RequireComponent(typeof(SpriteRenderer))]
public class Parallax : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ObstaculoSpawner spawner;

    [Header("Movimiento")]
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

    private void Update()
    {
        Vector3 posicion = transform.position;
        posicion.x -= spawner.VelocidadEscenario * factor * Time.deltaTime;
        // Al correrse una tira entera vuelve atras de golpe: como el dibujo se repite, no se nota el salto.
        if (posicion.x <= xInicial - anchoTira) posicion.x += anchoTira;
        transform.position = posicion;
    }
}
