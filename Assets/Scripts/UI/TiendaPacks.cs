using UnityEngine;

// La pestana de la tienda que vende monedas y gemas por dinero real. Es la unica parte del juego
// que cobra plata de verdad, por eso nunca compra de un toque: primero pasa un adulto por la puerta,
// despues cobra el backend y recien con la respuesta se acredita y se guarda.
public class TiendaPacks : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Wallet billetera;
    [SerializeField] private ConfirmDialog confirmacion;
    [SerializeField] private PuertaAdultos puerta;
    // Unity no muestra interfaces en el Inspector: se arrastra el componente que la implemente
    // (hoy BackendSimulado) y aca se usa solo como IPurchaseBackend.
    [SerializeField] private MonoBehaviour backend;
    // Un PackSlot apagado que se copia por cada paquete, igual que en ShopShelf.
    [SerializeField] private PackSlot plantilla;
    [SerializeField] private Transform contenedor;
    // Tapa la tienda mientras la compra esta en curso, asi no se puede pedir otra encima.
    [SerializeField] private GameObject procesando;

    [Header("Paquetes")]
    [SerializeField] private PackDefinition[] packs;
    [SerializeField] private Sprite iconoMonedas;
    [SerializeField] private Sprite iconoGemas;

    [Header("Textos")]
    // Mientras el backend sea el simulado, el que aprueba tiene que saber que no se le cobra nada.
    // Con la tienda real se deja vacio.
    [SerializeField] private string leyendaPrueba = "Compra de prueba: no se cobra dinero real.";

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoCompra;

    private IPurchaseBackend cobrador;
    private bool comprando;

    private void Awake()
    {
        cobrador = backend as IPurchaseBackend;
        if (cobrador == null) Debug.LogError("TiendaPacks: el backend no implementa IPurchaseBackend.", this);

        plantilla.gameObject.SetActive(false);
        procesando.SetActive(false);
        foreach (PackDefinition pack in packs)
        {
            if (pack == null) continue;
            PackSlot slot = Instantiate(plantilla, contenedor);
            slot.gameObject.SetActive(true);
            slot.name = pack.Id;
            slot.Mostrar(pack, Icono(pack.Moneda), Pedir);
        }
    }

    private Sprite Icono(CurrencyType moneda) => moneda == CurrencyType.Gemas ? iconoGemas : iconoMonedas;

    private static string NombreMoneda(CurrencyType moneda) => moneda == CurrencyType.Gemas ? "gemas" : "monedas";

    private string Leyenda() => string.IsNullOrEmpty(leyendaPrueba) ? "" : "\n<size=75%>" + leyendaPrueba + "</size>";

    private void Pedir(PackDefinition pack)
    {
        if (comprando) return;
        if (cobrador == null || !cobrador.Listo)
        {
            confirmacion.Avisar("La tienda no está disponible ahora.\n<size=75%>Probá de nuevo más tarde.</size>");
            return;
        }

        string queSeCompra = pack.Nombre + ": " + pack.Cantidad + " " + NombreMoneda(pack.Moneda) + " por " + pack.PrecioVitrina + Leyenda();
        puerta.Pedir(queSeCompra, () => Cobrar(pack));
    }

    private void Cobrar(PackDefinition pack)
    {
        comprando = true;
        procesando.SetActive(true);
        cobrador.Comprar(pack, resultado => Terminar(pack, resultado));
    }

    private void Terminar(PackDefinition pack, ResultadoCompra resultado)
    {
        comprando = false;
        procesando.SetActive(false);

        switch (resultado)
        {
            case ResultadoCompra.Exito:
                billetera.Sumar(pack.Moneda, pack.Cantidad);
                // Se guarda en el momento: si la app se cierra ahora, lo pagado no se puede perder.
                SaveSystem.Guardar();
                SoundPlayer.Reproducir(sonidoCompra);
                confirmacion.Avisar("¡Listo! Sumaste " + pack.Cantidad + " " + NombreMoneda(pack.Moneda) + "." + Leyenda());
                break;
            case ResultadoCompra.Fallo:
                confirmacion.Avisar("No se pudo hacer la compra y no se cobró nada.\n<size=75%>Probá de nuevo más tarde.</size>");
                break;
            case ResultadoCompra.Cancelado:
                confirmacion.Avisar("Compra cancelada. No se cobró nada.");
                break;
        }
    }
}
