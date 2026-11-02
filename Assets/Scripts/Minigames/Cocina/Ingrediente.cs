using TMPro;
using UnityEngine;

// Un ingrediente del estante. Se arrastra con el mismo Draggable de la casa y, si se suelta sobre
// la olla, entra. Solo se usan los que se compraron en la tienda: muestra cuantos quedan y, sin
// stock, se apaga y no se puede agarrar.
[RequireComponent(typeof(Draggable))]
public class Ingrediente : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private IngredientDefinition definicion;
    [SerializeField] private Olla olla;
    [SerializeField] private TMP_Text cantidad;

    [Header("Colores")]
    [SerializeField] private Color colorSinStock = new Color(1f, 1f, 1f, 0.3f);

    private Draggable arrastre;
    private SpriteRenderer dibujo;

    // Lo que queda para tirar: lo comprado menos el que ya esta en la olla (entra uno de cada uno).
    // No se gasta al tirarlo: se gasta recien al cocinar una receta.
    private int Disponibles => Despensa.Ingredientes.Cantidad(definicion.Id) - (olla.Tiene(definicion) ? 1 : 0);

    private void Awake()
    {
        arrastre = GetComponent<Draggable>();

        // El dibujo sale del dato: si arte cambia el sprite del ingrediente, el estante se entera solo.
        dibujo = GetComponent<SpriteRenderer>();
        if (dibujo != null && definicion != null) dibujo.sprite = definicion.Sprite;
    }

    private void OnEnable()
    {
        arrastre.Soltado += AlSoltar;
        olla.ContenidoCambiado += Actualizar;
        Despensa.Ingredientes.Cambio += AlCambiarInventario;
        Actualizar();
    }

    private void OnDisable()
    {
        arrastre.Soltado -= AlSoltar;
        olla.ContenidoCambiado -= Actualizar;
        Despensa.Ingredientes.Cambio -= AlCambiarInventario;
    }

    private void AlCambiarInventario(string id, int cantidadNueva)
    {
        if (id == definicion.Id) Actualizar();
    }

    private void Actualizar()
    {
        int quedan = Mathf.Max(0, Disponibles);
        if (cantidad != null) cantidad.SetText("x{0}", quedan);
        if (dibujo != null) dibujo.color = quedan > 0 ? Color.white : colorSinStock;
        // Con el Draggable apagado no lee toques: sin stock no se puede ni levantar.
        arrastre.enabled = quedan > 0;
    }

    private void AlSoltar(Vector2 puntoMundo)
    {
        if (Disponibles > 0 && olla.Contiene(puntoMundo)) olla.Agregar(definicion);
    }
}
