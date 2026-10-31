using System.Collections.Generic;
using UnityEngine;

/// El estante de una pestana de la tienda: arma un ShopSlot por cada producto de su categoria y
/// cobra cuando el jugador confirma la compra.
public class ShopShelf : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Inventory inventario;
    [SerializeField] private Wallet billetera;
    [SerializeField] private ConfirmDialog confirmacion;
    // Un ShopSlot apagado que se copia por cada producto, asi el diseno se toca en la escena.
    [SerializeField] private ShopSlot plantilla;
    [SerializeField] private Transform contenedor;

    [Header("Estante")]
    [SerializeField] private ItemCategory categoria;
    [SerializeField] private Sprite iconoMonedas;
    [SerializeField] private Sprite iconoGemas;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoCompra;

    private readonly List<ShopSlot> slots = new List<ShopSlot>();
    private readonly List<ItemDefinition> productos = new List<ItemDefinition>();

    private void Awake()
    {
        plantilla.gameObject.SetActive(false);
        foreach (ItemDefinition item in inventario.Catalogo.Items)
        {
            if (item == null || item.Categoria != categoria || item.Precio <= 0) continue;
            ShopSlot slot = Instantiate(plantilla, contenedor);
            slot.gameObject.SetActive(true);
            slot.name = item.Id;
            slot.Mostrar(item, item.Moneda == CurrencyType.Gemas ? iconoGemas : iconoMonedas, Pedir);
            slots.Add(slot);
            productos.Add(item);
        }
    }

    private void OnEnable()
    {
        inventario.Cambiado += Refrescar;
        billetera.SaldoCambiado += AlCambiarSaldo;
        Refrescar();
    }

    private void OnDisable()
    {
        inventario.Cambiado -= Refrescar;
        billetera.SaldoCambiado -= AlCambiarSaldo;
    }

    private void AlCambiarSaldo(CurrencyType tipo, int saldo) => Refrescar();

    private void Refrescar()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Refrescar(inventario.Cantidad(productos[i]), inventario.PuedeComprar(productos[i]));
        }
    }

    private void Pedir(ItemDefinition item)
    {
        string moneda = item.Moneda == CurrencyType.Gemas ? "gemas" : "monedas";
        int falta = item.Precio - billetera.Obtener(item.Moneda);
        if (falta > 0)
        {
            confirmacion.Avisar("Te faltan " + falta + " " + moneda + " para " + item.Nombre + ".\n<size=75%>¡Jugá minijuegos para ganar más!</size>");
            return;
        }

        confirmacion.PreguntarCompra(item, item.Moneda == CurrencyType.Gemas ? iconoGemas : iconoMonedas, () =>
        {
            if (inventario.Comprar(item)) SoundPlayer.Reproducir(sonidoCompra);
        });
    }
}
