using UnityEngine;

/// Comida de la cocina: se arrastra hasta la mascota y al soltarla encima sube la necesidad que
/// diga el ItemDefinition. Si se suelta en otro lado, Draggable la devuelve a su lugar.
[RequireComponent(typeof(Draggable))]
public class FoodItem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ItemDefinition item;
    [SerializeField] private PetDropTarget mascota;
    [SerializeField] private PetMoodController cara;

    [Header("Feedback")]
    [SerializeField] private float segundosContento = 1.5f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoComer;

    private Draggable draggable;

    private void Awake()
    {
        draggable = GetComponent<Draggable>();
    }

    private void OnEnable()
    {
        draggable.Soltado += AlSoltar;
    }

    private void OnDisable()
    {
        draggable.Soltado -= AlSoltar;
    }

    private void AlSoltar(Vector2 puntoMundo)
    {
        if (item == null || mascota == null) return;
        if (!mascota.Contiene(puntoMundo)) return;

        mascota.Aplicar(item.Necesidad, item.Cantidad);
        // Que comer se note: el sonido, la cara y (en NeedBar) la barra que late.
        SoundPlayer.Reproducir(sonidoComer);
        if (cara != null) cara.Alegrar(segundosContento);
    }
}
