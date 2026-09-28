using UnityEngine;

/// Pelota de la sala de juegos: se agarra, se tira con el dedo y rebota contra los bordes y contra
/// Poku. Cada vez que le pega a Poku lo divierte y lo pone contento un par de segundos.
[RequireComponent(typeof(Draggable))]
[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PetDropTarget mascota;
    [SerializeField] private PetMoodController cara;

    [Header("Tiro")]
    [SerializeField] private float velocidadMaxima = 18f;
    // Si el dedo quedo quieto un rato antes de soltar, no es un tiro: la pelota cae sola.
    [SerializeField] private float quietoParaSoltar = 0.1f;

    [Header("Premio")]
    [SerializeField] private float diversionPorGolpe = 4f;
    [SerializeField] private float segundosContento = 2f;
    // Un rebote apoyado genera varios contactos seguidos: sin esta pausa contarian como muchos golpes.
    [SerializeField] private float pausaEntreGolpes = 0.5f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoRebote;
    // Apoyada en el piso genera contactos chiquitos todo el tiempo: esos no suenan.
    [SerializeField] private float velocidadParaSonar = 2f;

    private Draggable draggable;
    private Rigidbody2D body;
    private Vector2 ultimaPosicion;
    private Vector2 velocidadDedo;
    private float ultimoMovimiento;
    private float ultimoGolpe = -10f;

    private void Awake()
    {
        draggable = GetComponent<Draggable>();
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        draggable.Levantado += AlLevantar;
        draggable.Movido += AlMover;
        draggable.Soltado += AlSoltar;
    }

    private void OnDisable()
    {
        draggable.Levantado -= AlLevantar;
        draggable.Movido -= AlMover;
        draggable.Soltado -= AlSoltar;
    }

    private void Update()
    {
        // Si el arrastre se corta sin soltar (una llamada, o se abre una pantalla encima), la pelota
        // no puede quedar colgada en el aire: vuelve a tener fisica.
        if (body.bodyType == RigidbodyType2D.Kinematic && !draggable.Arrastrando) Soltar(Vector2.zero);
    }

    private void AlLevantar()
    {
        // Mientras se arrastra la mueve el dedo, no la gravedad ni los choques.
        body.bodyType = RigidbodyType2D.Kinematic;
        body.linearVelocity = Vector2.zero;
        body.angularVelocity = 0f;
        ultimaPosicion = transform.position;
        velocidadDedo = Vector2.zero;
        ultimoMovimiento = Time.time;
    }

    private void AlMover(Vector2 puntoMundo)
    {
        Vector2 posicion = transform.position;
        float dt = Time.time - ultimoMovimiento;
        if (dt > 0f)
        {
            // Promedio que pesa mas lo ultimo, como el suavizado de la clase 5: un temblor de un
            // frame no decide el tiro.
            velocidadDedo = Vector2.Lerp(velocidadDedo, (posicion - ultimaPosicion) / dt, 0.5f);
        }
        ultimaPosicion = posicion;
        ultimoMovimiento = Time.time;
    }

    private void AlSoltar(Vector2 puntoMundo)
    {
        bool quieto = Time.time - ultimoMovimiento > quietoParaSoltar;
        Soltar(quieto ? Vector2.zero : velocidadDedo);
    }

    private void Soltar(Vector2 velocidad)
    {
        body.bodyType = RigidbodyType2D.Dynamic;
        body.linearVelocity = Vector2.ClampMagnitude(velocidad, velocidadMaxima);
    }

    private void OnCollisionEnter2D(Collision2D choque)
    {
        // Suena contra cualquier cosa (piso, paredes, Poku), no solo cuando lo divierte.
        if (choque.relativeVelocity.magnitude >= velocidadParaSonar) SoundPlayer.Reproducir(sonidoRebote);

        if (choque.gameObject != mascota.gameObject) return;
        if (Time.time - ultimoGolpe < pausaEntreGolpes) return;
        ultimoGolpe = Time.time;

        mascota.Aplicar(NeedType.Diversion, diversionPorGolpe);
        cara.Alegrar(segundosContento);
    }
}
