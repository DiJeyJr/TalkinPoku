using UnityEngine;

// ============================================================================
// Ball.cs
// QUE HACE: la pelota de la Sala de juegos. Se agarra, se tira con el dedo y
//           rebota. Cada vez que le pega a Poku le sube la diversion, lo pone
//           contento y le da experiencia.
// DONDE ESTA: en "Habitaciones/Sala de juegos/Pelota" de SampleScene.
// PARA CAMBIAR (Inspector de la Pelota):
//   - cuanta diversion da cada golpe -> diversionPorGolpe
//   - cuanto tiempo queda contento -> segundosContento
//   - que tan fuerte se puede tirar -> velocidadMaxima
//   - cada cuanto puede contar otro golpe -> pausaEntreGolpes
//   - cuanta XP da -> ProgressionConfig.asset (fuente "Pelota")
// ============================================================================

// Pelota de la sala de juegos: se agarra, se tira con el dedo y rebota contra los bordes y contra
// Poku. Cada vez que le pega a Poku lo divierte y lo pone contento un par de segundos.
[RequireComponent(typeof(Draggable))]
[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PetDropTarget mascota;
    [SerializeField] private PetMoodController cara;
    [SerializeField] private Progression progreso;

    [Header("Tiro")]
    // [AJUSTE] Velocidad maxima con la que sale la pelota al tirarla.
    [SerializeField] private float velocidadMaxima = 18f;
    // Si el dedo quedo quieto un rato antes de soltar, no es un tiro: la pelota cae sola.
    [SerializeField] private float quietoParaSoltar = 0.1f;

    [Header("Premio")]
    // [AJUSTE] Diversion que suma cada golpe a Poku.
    [SerializeField] private float diversionPorGolpe = 4f;
    // [AJUSTE] Segundos que Poku queda con cara contenta.
    [SerializeField] private float segundosContento = 2f;
    // [AJUSTE] Segundos minimos entre dos golpes que cuentan.
    // Un rebote apoyado genera varios contactos seguidos: sin esta pausa contarian como muchos golpes.
    [SerializeField] private float pausaEntreGolpes = 0.5f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoRebote;
    // Apoyada en el piso genera contactos chiquitos todo el tiempo: esos no suenan.
    [SerializeField] private float velocidadParaSonar = 2f;

    private Draggable draggable;
    private Rigidbody2D body;
    private Vector2 ultimaPosicion;
    // Velocidad del dedo mientras arrastra (es la que se le da a la pelota al soltar).
    private Vector2 velocidadDedo;
    private float ultimoMovimiento;
    private float ultimoGolpe = -10f;

    private void Awake()
    {
        draggable = GetComponent<Draggable>();
        body = GetComponent<Rigidbody2D>();
    }

    // Se engancha a los eventos de arrastre.
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

    // Al agarrarla: se apaga la fisica.
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

    // Mientras se mueve: calcula la velocidad del dedo.
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

    // Al soltarla: sale con la velocidad del dedo (o cae sola si el dedo estaba quieto).
    private void AlSoltar(Vector2 puntoMundo)
    {
        bool quieto = Time.time - ultimoMovimiento > quietoParaSoltar;
        Soltar(quieto ? Vector2.zero : velocidadDedo);
    }

    // Vuelve a prender la fisica y le da la velocidad (limitada a velocidadMaxima).
    private void Soltar(Vector2 velocidad)
    {
        body.bodyType = RigidbodyType2D.Dynamic;
        body.linearVelocity = Vector2.ClampMagnitude(velocidad, velocidadMaxima);
    }

    // Unity lo llama cuando la pelota choca con algo. Si es Poku, le da el premio.
    private void OnCollisionEnter2D(Collision2D choque)
    {
        // Suena contra cualquier cosa (piso, paredes, Poku), no solo cuando lo divierte.
        if (choque.relativeVelocity.magnitude >= velocidadParaSonar) SoundPlayer.Reproducir(sonidoRebote);

        if (choque.gameObject != mascota.gameObject) return;
        if (Time.time - ultimoGolpe < pausaEntreGolpes) return;
        ultimoGolpe = Time.time;

        // Premio del golpe: diversion, cara contenta y experiencia.
        mascota.Aplicar(NeedType.Diversion, diversionPorGolpe);
        cara.Alegrar(segundosContento);
        if (progreso != null) progreso.Ganar(XpSource.Pelota);
    }
}
