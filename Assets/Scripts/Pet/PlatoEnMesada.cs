using TMPro;
using UnityEngine;

/// Un lugar de la mesada de la cocina. Muestra un plato del inventario y cuantos hay; arrastrado
/// hasta Poku, se lo come y le da la energia de la receta. Que plato muestra lo decide la Mesada.
[RequireComponent(typeof(Draggable))]
public class PlatoEnMesada : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PetDropTarget mascota;
    [SerializeField] private TMP_Text cantidad;

    private Draggable arrastre;
    private SpriteRenderer dibujo;
    private RecipeDefinition receta;

    private void Awake()
    {
        arrastre = GetComponent<Draggable>();
        dibujo = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        arrastre.Soltado += AlSoltar;
    }

    private void OnDisable()
    {
        arrastre.Soltado -= AlSoltar;
    }

    // null deja el lugar vacio.
    public void Mostrar(RecipeDefinition plato)
    {
        receta = plato;
        gameObject.SetActive(plato != null);
        if (plato == null) return;

        dibujo.sprite = plato.Sprite;
        cantidad.SetText("x{0}", Despensa.Platos.Cantidad(plato.Id));
    }

    private void AlSoltar(Vector2 puntoMundo)
    {
        if (receta == null || !mascota.Contiene(puntoMundo)) return;

        // Copia local: Gastar avisa a la Mesada, que reacomoda los lugares y, si era el ultimo, deja
        // este vacio (receta en null) antes de que termine este metodo.
        RecipeDefinition comido = receta;
        if (!Despensa.Platos.Gastar(comido.Id)) return;

        mascota.Comer(NeedType.Energia, comido.Energia);
        // El plato ya no esta en el inventario: que no vuelva si la app se cierra antes del autoguardado.
        SaveSystem.Guardar();
    }
}
