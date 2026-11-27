using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ============================================================================
// ShopShelf.cs
// QUE HACE: una pestana de la tienda (Comida, Ropa o Decoraciones). Crea un
//           casillero (ShopSlot) por cada item de su categoria, ordenados por
//           nivel, y maneja la compra: avisa si esta bloqueado o si no alcanza
//           la plata, y si alcanza pide confirmacion y cobra.
// DONDE ESTA: en "Canvas/Pantallas/Tienda/Contenido/Paneles/PanelComida",
//             "PanelRopa" y "PanelDecoraciones" de SampleScene.
// PARA CAMBIAR: los textos de los avisos -> Pedir().
//               que items aparecen -> el catalogo (Data/Items/ItemCatalog.asset);
//               aparecen los de la categoria de esta pestana con precio > 0.
// ============================================================================

// El estante de una pestana de la tienda: arma un ShopSlot por cada producto de su categoria y
// cobra cuando el jugador confirma la compra.
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
    // Que categoria de items vende esta pestana.
    [SerializeField] private ItemCategory categoria;
    [SerializeField] private Sprite iconoMonedas;
    [SerializeField] private Sprite iconoGemas;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoCompra;

    private readonly List<ShopSlot> slots = new List<ShopSlot>();
    private readonly List<ItemDefinition> productos = new List<ItemDefinition>();

    // Arma los casilleros una sola vez.
    private void Awake()
    {
        plantilla.gameObject.SetActive(false);
        // Por nivel, asi lo que ya puede comprar queda primero y los candados al final. OrderBy no
        // desordena los del mismo nivel: entre ellos queda el orden del catalogo.
        IEnumerable<ItemDefinition> ordenados = inventario.Catalogo.Items
            .Where(item => item != null && item.Categoria == categoria && item.Precio > 0)
            .OrderBy(item => item.NivelRequerido);

        foreach (ItemDefinition item in ordenados)
        {
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

    // Actualiza cada casillero (si ya lo tiene, si le alcanza, candado).
    private void Refrescar()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Refrescar(inventario.Cantidad(productos[i]), inventario.PuedeComprar(productos[i]));
        }
    }

    // Tocaron un producto: bloqueado -> aviso de nivel; no alcanza -> aviso; si no, confirmar y comprar.
    private void Pedir(ItemDefinition item)
    {
        if (item.Bloqueado)
        {
            confirmacion.Avisar(item.Nombre + " se desbloquea en el nivel " + item.NivelRequerido + ".\n<size=75%>¡Cuidá a Poku para subir de nivel!</size>");
            return;
        }

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
