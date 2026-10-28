using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Un producto en el estante de la tienda: icono, nombre, precio y si ya lo tiene. No compra: avisa
/// al estante, que es el que pregunta y cobra.
public class ShopSlot : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image icono;
    [SerializeField] private TMP_Text nombre;
    [SerializeField] private TMP_Text precio;
    [SerializeField] private Image iconoMoneda;
    [SerializeField] private TMP_Text estado;
    [SerializeField] private Button boton;

    private ItemDefinition item;
    private Action<ItemDefinition> alTocar;

    private void Awake()
    {
        boton.onClick.AddListener(() => alTocar?.Invoke(item));
    }

    public void Mostrar(ItemDefinition nuevo, Sprite moneda, Action<ItemDefinition> tocado)
    {
        item = nuevo;
        alTocar = tocado;
        icono.sprite = item.Icono;
        icono.preserveAspect = true;
        nombre.SetText(item.Nombre);
        precio.SetText("{0}", item.Precio);
        iconoMoneda.sprite = moneda;
    }

    public void Refrescar(int cantidad, bool comprable)
    {
        // La comida muestra cuantas tiene; ropa y muebles, que ya estan comprados.
        if (item.SeAcumula) estado.SetText(cantidad > 0 ? "Tenés " + cantidad : "");
        else estado.SetText(cantidad > 0 ? "¡Ya es tuyo!" : "");

        // Lo que no le alcanza se ve apagado pero se puede tocar: el cartel le dice cuanto le falta.
        bool yaLoTiene = !item.SeAcumula && cantidad > 0;
        boton.interactable = !yaLoTiene;
        icono.color = comprable || yaLoTiene ? Color.white : new Color(1f, 1f, 1f, 0.55f);
        precio.gameObject.SetActive(!yaLoTiene);
        iconoMoneda.gameObject.SetActive(!yaLoTiene);
    }
}
