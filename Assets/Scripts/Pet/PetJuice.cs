using UnityEngine;

/// Movimiento de Poku por codigo, sin Animator: se aplasta al tocarlo, rebota al comer, se sacude
/// cuando esta enojado o sucio y respira en reposo. Va en el mismo objeto que PetEvolution, que
/// tambien escribe la escala; los otros scripts solo llaman a Aplastar, Rebotar y Sacudir.
public class PetJuice : MonoBehaviour
{
    [Header("Aplastar (tap y caricia)")]
    [SerializeField] private float aplastarAmplitud = 0.2f;
    [SerializeField] private float aplastarDuracion = 0.4f;
    [SerializeField] private float aplastarOscilaciones = 1.5f;
    [SerializeField] private float aplastarAmortiguacion = 3.5f;

    [Header("Rebote (comer)")]
    [SerializeField] private float reboteAmplitud = 0.15f;
    [SerializeField] private float reboteDuracion = 0.6f;
    [SerializeField] private float reboteOscilaciones = 2.5f;
    [SerializeField] private float reboteAmortiguacion = 3f;

    [Header("Sacudida (enojado o sucio)")]
    [SerializeField] private float sacudidaGrados = 6f;
    [SerializeField] private float sacudidaDuracion = 0.45f;
    [SerializeField] private float sacudidaFrecuencia = 14f;

    [Header("Respirar")]
    [SerializeField] private float respirarAmplitud = 0.02f;
    [SerializeField, Min(0.1f)] private float respirarPeriodo = 3.2f;

    private Vector3 escalaBase;
    private Quaternion rotacionBase;
    // Lo ultimo que escribi yo. Si la escala es otra, la cambio PetEvolution y esa pasa a ser la base.
    private Vector3 ultimaEscala;

    // Segundos desde que arranco cada reaccion. Infinito = no esta corriendo.
    private float tAplastar = float.PositiveInfinity;
    private float tRebote = float.PositiveInfinity;
    private float tSacudida = float.PositiveInfinity;
    private float tRespirar;
    private float fuerzaAplastar = 1f;

    private void Awake()
    {
        escalaBase = transform.localScale;
        rotacionBase = transform.localRotation;
    }

    private void OnEnable()
    {
        ultimaEscala = transform.localScale;
    }

    private void OnDisable()
    {
        // Sin esto, desactivarlo en medio de una reaccion dejaria a Poku torcido o aplastado.
        if (transform.localScale == ultimaEscala) transform.localScale = escalaBase;
        transform.localRotation = rotacionBase;
        tAplastar = tRebote = tSacudida = float.PositiveInfinity;
    }

    // Se aplasta en vertical y se ensancha; vuelve con un pequeno rebote. La intensidad deja usar
    // la misma reaccion mas suave (la caricia) sin otra perilla.
    public void Aplastar(float intensidad = 1f)
    {
        fuerzaAplastar = intensidad;
        tAplastar = 0f;
    }

    // Lo contrario de Aplastar: arranca estirado, como al abrir la boca, y se asienta masticando.
    public void Rebotar()
    {
        tRebote = 0f;
    }

    public void Sacudir()
    {
        tSacudida = 0f;
    }

    // LateUpdate y no Update: asi corre despues del pop de PetEvolution y se compone sobre el.
    private void LateUpdate()
    {
        if (transform.localScale != ultimaEscala) escalaBase = transform.localScale;

        float dt = Time.deltaTime;
        tAplastar += dt;
        tRebote += dt;
        tSacudida += dt;
        tRespirar = Mathf.Repeat(tRespirar + dt, respirarPeriodo);

        float y = 1f;
        y *= 1f - aplastarAmplitud * fuerzaAplastar *
            Oscilar(tAplastar, aplastarDuracion, aplastarOscilaciones, aplastarAmortiguacion);
        y *= 1f + reboteAmplitud * Oscilar(tRebote, reboteDuracion, reboteOscilaciones, reboteAmortiguacion);
        y *= 1f + respirarAmplitud * Mathf.Sin(2f * Mathf.PI * tRespirar / respirarPeriodo);

        // El ancho es la inversa del alto: el volumen se conserva y se lee como algo blando, no como
        // un sprite que se estira.
        ultimaEscala = new Vector3(escalaBase.x / y, escalaBase.y * y, escalaBase.z);
        transform.localScale = ultimaEscala;

        transform.localRotation = rotacionBase * Quaternion.Euler(0f, 0f, Sacudida());
    }

    // Arranca en el pico, asi la respuesta se ve en el mismo frame del toque, y oscila hasta
    // apagarse. Cruza por cero y pasa al lado contrario: eso es el rebote.
    private static float Oscilar(float t, float duracion, float oscilaciones, float amortiguacion)
    {
        if (t >= duracion) return 0f;
        float u = t / duracion;
        return Mathf.Exp(-amortiguacion * u) * Mathf.Cos(2f * Mathf.PI * oscilaciones * u);
    }

    private float Sacudida()
    {
        if (tSacudida >= sacudidaDuracion) return 0f;
        float u = tSacudida / sacudidaDuracion;
        float apagado = (1f - u) * (1f - u);
        return sacudidaGrados * apagado * Mathf.Sin(2f * Mathf.PI * sacudidaFrecuencia * tSacudida);
    }
}
