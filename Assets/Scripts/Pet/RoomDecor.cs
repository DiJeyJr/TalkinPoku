using System.Collections.Generic;
using UnityEngine;

/// Va en la sala. Dibuja los muebles que el jugador coloco y, mientras esta abierta la bandeja de
/// decoracion, deja arrastrarlos para acomodarlos. Con la bandeja cerrada quedan quietos: si no,
/// tocar un mueble al deslizar para cambiar de habitacion lo moveria.
public class RoomDecor : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Inventory inventario;
    [SerializeField] private InventoryTray bandeja;
    // El fondo de la sala: los muebles van en su capa de dibujo, encima de el.
    [SerializeField] private SpriteRenderer fondo;

    [Header("Limites")]
    // Hasta donde se puede llevar un mueble, para que no se pierda fuera de la pantalla.
    [SerializeField] private Rect zona = new Rect(-4.5f, -9f, 9f, 13f);

    private readonly Dictionary<string, Draggable> muebles = new Dictionary<string, Draggable>();
    private bool editando;

    private void OnEnable()
    {
        inventario.Cambiado += Refrescar;
        bandeja.AbiertaCambiada += AlAbrirBandeja;
        Refrescar();
    }

    private void OnDisable()
    {
        inventario.Cambiado -= Refrescar;
        bandeja.AbiertaCambiada -= AlAbrirBandeja;
    }

    private void AlAbrirBandeja(bool abierta)
    {
        editando = abierta;
        foreach (Draggable mueble in muebles.Values) mueble.enabled = editando;
    }

    private void Refrescar()
    {
        // Saca los que ya no estan colocados.
        List<string> sacar = new List<string>();
        foreach (KeyValuePair<string, Draggable> par in muebles)
        {
            ItemDefinition item = inventario.Catalogo.Buscar(par.Key);
            if (item == null || !inventario.Colocado(item)) sacar.Add(par.Key);
        }
        foreach (string id in sacar)
        {
            Destroy(muebles[id].gameObject);
            muebles.Remove(id);
        }

        foreach (ItemDefinition item in inventario.Catalogo.Items)
        {
            if (item == null || item.Categoria != ItemCategory.Decoracion) continue;
            if (!inventario.Colocado(item) || muebles.ContainsKey(item.Id)) continue;
            muebles.Add(item.Id, Crear(item));
        }
    }

    private Draggable Crear(ItemDefinition item)
    {
        GameObject objeto = new GameObject(item.Id);
        objeto.transform.SetParent(transform, false);
        objeto.transform.position = inventario.PosicionDe(item);
        objeto.transform.localScale = Vector3.one * item.EscalaMundo;

        SpriteRenderer dibujo = objeto.AddComponent<SpriteRenderer>();
        dibujo.sprite = item.SpriteMundo;
        dibujo.sortingLayerID = fondo.sortingLayerID;
        dibujo.sortingOrder = fondo.sortingOrder + 1 + item.Capa;

        objeto.AddComponent<BoxCollider2D>();
        Draggable arrastre = objeto.AddComponent<Draggable>();
        arrastre.VolverAlOrigen = false;
        arrastre.enabled = editando;
        arrastre.Soltado += punto => AlSoltar(item, objeto.transform);
        return arrastre;
    }

    private void AlSoltar(ItemDefinition item, Transform mueble)
    {
        Vector2 p = mueble.position;
        p.x = Mathf.Clamp(p.x, zona.xMin, zona.xMax);
        p.y = Mathf.Clamp(p.y, zona.yMin, zona.yMax);
        mueble.position = p;
        inventario.Mover(item, p);
    }
}
