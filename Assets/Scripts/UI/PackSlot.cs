using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Un paquete en la tienda de dinero real: cuanto trae y cuanto sale. Como ShopSlot, no compra: avisa
// a TiendaPacks, que pide la puerta para adultos y cobra.
public class PackSlot : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image icono;
    [SerializeField] private TMP_Text nombre;
    [SerializeField] private TMP_Text cantidad;
    [SerializeField] private Image iconoMoneda;
    [SerializeField] private TMP_Text precio;
    [SerializeField] private Button boton;

    private PackDefinition pack;
    private Action<PackDefinition> alTocar;

    private void Awake()
    {
        boton.onClick.AddListener(() => alTocar?.Invoke(pack));
    }

    public void Mostrar(PackDefinition nuevo, Sprite iconoDeMoneda, Action<PackDefinition> tocado)
    {
        pack = nuevo;
        alTocar = tocado;
        // Sin icono propio queda el de la plantilla.
        if (pack.Icono != null)
        {
            icono.sprite = pack.Icono;
            icono.preserveAspect = true;
        }
        nombre.SetText(pack.Nombre);
        cantidad.SetText("{0}", pack.Cantidad);
        iconoMoneda.sprite = iconoDeMoneda;
        precio.SetText(pack.PrecioVitrina);
    }
}
