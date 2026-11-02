using UnityEngine;

// Va en la mascota. Las actividades le aplican efectos por aca, asi ninguna necesita conocer al
// NeedsSystem, y lo que pasa cuando come esta escrito una sola vez.
[RequireComponent(typeof(Collider2D))]
public class PetDropTarget : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    // Opcional: sin ella comer no da experiencia.
    [SerializeField] private Progression progreso;

    [Header("Feedback")]
    [SerializeField] private float segundosContento = 1.5f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoComer;

    private Collider2D miCollider;
    // Estan en el mismo objeto que este componente: se buscan solos y no hay que arrastrarlos.
    private PetMoodController cara;
    private PetJuice juice;

    private void Awake()
    {
        miCollider = GetComponent<Collider2D>();
        cara = GetComponent<PetMoodController>();
        juice = GetComponent<PetJuice>();
    }

    public bool Contiene(Vector2 puntoMundo)
    {
        return miCollider.OverlapPoint(puntoMundo);
    }

    public void Aplicar(NeedType necesidad, float cantidad)
    {
        necesidades.Sumar(necesidad, cantidad);
    }

    // La comida de la cocina, los platos de la mesada y la bandeja terminan aca: que comer se note
    // igual venga de donde venga (el sonido, la cara, el rebote y, en NeedBar, la barra que late).
    public void Comer(NeedType necesidad, float cantidad)
    {
        Aplicar(necesidad, cantidad);
        SoundPlayer.Reproducir(sonidoComer);
        if (cara != null) cara.Alegrar(segundosContento);
        if (progreso != null) progreso.Ganar(XpSource.Comer);
        if (juice != null) juice.Rebotar();
        Vibracion.Vibrar();
    }
}
