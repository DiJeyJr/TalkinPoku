using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// Un objeto en la bandeja de inventario. La ropa y los muebles se tocan para ponerlos o sacarlos;
/// la comida se arrastra hasta Poku. El arrastre lo maneja la bandeja: aca solo se avisa.
public class InventorySlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Referencias")]
    [SerializeField] private Image icono;
    [SerializeField] private TMP_Text nombre;
    [SerializeField] private TMP_Text cantidad;
    // Puesto o colocado se ve con el marco de "Equipado", que ya trae el cartel dibujado.
    [SerializeField] private Image marco;
    [SerializeField] private Sprite marcoNormal;
    [SerializeField] private Sprite marcoEquipado;

    public ItemDefinition Item { get; private set; }

    public event Action<InventorySlot> Tocado;
    public event Action<InventorySlot, PointerEventData> ArrastreEmpezado;
    public event Action<PointerEventData> Arrastrando;
    public event Action<InventorySlot, PointerEventData> Soltado;

    // Si el arrastre es de costado se le pasa a la lista para que se deslice, como cualquier lista
    // del celular. Solo un arrastre hacia arriba agarra la comida.
    private ScrollRect lista;
    private bool pasadoALista;

    private void Awake()
    {
        lista = GetComponentInParent<ScrollRect>();
    }

    public void Mostrar(ItemDefinition item, int cuantos, bool activo)
    {
        Item = item;
        icono.sprite = item.Icono;
        icono.preserveAspect = true;
        if (item.SeAcumula) cantidad.SetText("x{0}", cuantos);
        else cantidad.SetText("");
        nombre.SetText(item.Nombre);
        marco.sprite = activo ? marcoEquipado : marcoNormal;
    }

    public void OnPointerClick(PointerEventData evento)
    {
        if (evento.dragging) return;
        Tocado?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData evento)
    {
        Vector2 d = evento.delta;
        pasadoALista = !Item.SeAcumula || Mathf.Abs(d.x) > Mathf.Abs(d.y);
        if (pasadoALista)
        {
            if (lista != null) lista.OnBeginDrag(evento);
            return;
        }
        ArrastreEmpezado?.Invoke(this, evento);
    }

    public void OnDrag(PointerEventData evento)
    {
        if (pasadoALista)
        {
            if (lista != null) lista.OnDrag(evento);
            return;
        }
        Arrastrando?.Invoke(evento);
    }

    public void OnEndDrag(PointerEventData evento)
    {
        if (pasadoALista)
        {
            if (lista != null) lista.OnEndDrag(evento);
            return;
        }
        Soltado?.Invoke(this, evento);
    }
}
