using UnityEngine;

/// Un ingrediente bueno o un objeto malo que cae. Es siempre el mismo objeto reciclado: el spawner
/// le cambia el dibujo y si es bueno cada vez que lo vuelve a tirar.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ObjetoQueCae : MonoBehaviour
{
    public bool EsBueno { get; private set; }
    public Vector2 Posicion => body.position;

    private Rigidbody2D body;
    private SpriteRenderer dibujo;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        dibujo = GetComponent<SpriteRenderer>();
    }

    public void Caer(Vector3 posicion, Sprite sprite, bool esBueno, float velocidad, float giro)
    {
        // Se mueve antes de prenderlo: con el objeto apagado el Rigidbody2D no simula. Y el resto va
        // despues porque Awake recien corre la primera vez que se prende.
        transform.position = posicion;
        transform.rotation = Quaternion.identity;
        gameObject.SetActive(true);

        EsBueno = esBueno;
        dibujo.sprite = sprite;
        // Velocidad fija en vez de gravedad: asi la dificultad la maneja un solo numero y no depende
        // de desde que altura arranca.
        body.linearVelocity = new Vector2(0f, -velocidad);
        body.angularVelocity = giro;
    }

    public void Frenar()
    {
        // Uno que todavia no cayo nunca se prendio, asi que no paso por Awake y no tiene body.
        if (body == null) return;
        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;
    }
}
