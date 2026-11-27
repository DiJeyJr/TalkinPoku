using TMPro;
using UnityEngine;

// ============================================================================
// PlatoEnMesada.cs
// QUE HACE: un lugar de la mesada de la cocina. Muestra un plato cocinado y
//           cuantos hay. Si se arrastra hasta Poku, se lo come y le sube la
//           energia que diga la receta.
// DONDE ESTA: en "Habitaciones/Cocina/Mesada/Plato1", "Plato2" y "Plato3".
// PARA CAMBIAR: cuanta energia da cada plato -> se calcula como la suma de los
//               precios de sus ingredientes + "Energia Extra" del asset de la
//               receta (Data/Cocina/Receta_*). Que necesidad llena -> el
//               NeedType.Energia en AlSoltar().
// ============================================================================

// Un lugar de la mesada de la cocina. Muestra un plato del inventario y cuantos hay; arrastrado
// hasta Poku, se lo come y le da la energia de la receta. Que plato muestra lo decide la Mesada.
[RequireComponent(typeof(Draggable))]
public class PlatoEnMesada : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PetDropTarget mascota;
    // Texto "x3" con la cantidad.
    [SerializeField] private TMP_Text cantidad;

    private Draggable arrastre;
    private SpriteRenderer dibujo;
    // El plato que muestra ahora (null = vacio).
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

    // La llama la Mesada para decirle que plato mostrar.
    // null deja el lugar vacio.
    public void Mostrar(RecipeDefinition plato)
    {
        receta = plato;
        gameObject.SetActive(plato != null);
        if (plato == null) return;

        dibujo.sprite = plato.Sprite;
        cantidad.SetText("x{0}", Despensa.Platos.Cantidad(plato.Id));
    }

    // Si se solto sobre Poku: gasta un plato y Poku lo come.
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
