using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ============================================================================
// ShopSlot.cs
// QUE HACE: un producto en la tienda: icono, nombre, precio (o "Nivel N" con
//           candado si esta bloqueado) y si ya lo tiene. No compra: avisa al
//           estante (ShopShelf), que pregunta y cobra.
// DONDE ESTA: es la plantilla "Producto" dentro de cada panel de la tienda.
// PARA CAMBIAR: que tan apagado se ve lo que no alcanza -> el 0.55f en Refrescar().
//               el texto del candado -> "Nivel {0}" en Refrescar().
// ============================================================================

// Un producto en el estante de la tienda: icono, nombre, precio y si ya lo tiene. No compra: avisa
// al estante, que es el que pregunta y cobra.
public class ShopSlot : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image icono;
    [SerializeField] private TMP_Text nombre;
    [SerializeField] private TMP_Text precio;
    [SerializeField] private Image iconoMoneda;
    [SerializeField] private TMP_Text estado;
    [SerializeField] private Button boton;
    [SerializeField] private Image marco;

    [Header("Marcos")]
    // El de "Comprado" ya trae el cartel dibujado: con eso alcanza para ropa y muebles que ya tiene.
    [SerializeField] private Sprite marcoNormal;
    [SerializeField] private Sprite marcoComprado;
    // Va en lugar de la moneda mientras no tiene el nivel, con el nivel en lugar del precio.
    [SerializeField] private Sprite candado;

    private ItemDefinition item;
    private Sprite moneda;
    private Action<ItemDefinition> alTocar;

    private void Awake()
    {
        boton.onClick.AddListener(() => alTocar?.Invoke(item));
    }

    // Carga el item en el casillero (se llama una vez al armar la tienda).
    public void Mostrar(ItemDefinition nuevo, Sprite iconoDeMoneda, Action<ItemDefinition> tocado)
    {
        item = nuevo;
        alTocar = tocado;
        moneda = iconoDeMoneda;
        icono.sprite = item.Icono;
        icono.preserveAspect = true;
        nombre.SetText(item.Nombre);
    }

    // Actualiza el estado: comprado, alcanza o no, bloqueado por nivel.
    public void Refrescar(int cantidad, bool comprable)
    {
        // La comida muestra cuantas tiene; ropa y muebles, que ya estan comprados.
        if (item.SeAcumula) estado.SetText(cantidad > 0 ? "Tenés " + cantidad : "");
        else estado.SetText("");

        // Lo que no le alcanza se ve apagado pero se puede tocar: el cartel le dice cuanto le falta.
        bool yaLoTiene = !item.SeAcumula && cantidad > 0;
        boton.interactable = !yaLoTiene;
        marco.sprite = yaLoTiene ? marcoComprado : marcoNormal;
        icono.color = comprable || yaLoTiene ? Color.white : new Color(1f, 1f, 1f, 0.55f);
        // Se toca igual: el cartel le dice en que nivel se desbloquea.
        bool bloqueado = item.Bloqueado && !yaLoTiene;
        if (bloqueado) precio.SetText("Nivel {0}", item.NivelRequerido);
        else precio.SetText("{0}", item.Precio);
        iconoMoneda.sprite = bloqueado && candado != null ? candado : moneda;
        precio.gameObject.SetActive(!yaLoTiene);
        iconoMoneda.gameObject.SetActive(!yaLoTiene);
    }
}
