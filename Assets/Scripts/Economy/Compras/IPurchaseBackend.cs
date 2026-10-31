using System;

/// Quien cobra de verdad. La tienda de packs solo conoce esta interfaz, no a Google Play.
///
/// Hoy la implementa BackendSimulado. Para cobrar en serio se agrega el paquete de Unity IAP y una
/// clase nueva (por ejemplo BackendUnityIAP) que implemente esto mismo: en Comprar busca el producto
/// por pack.ProductId, lanza la compra y llama a alTerminar cuando la tienda responde. Despues se
/// arrastra esa en lugar del simulado en TiendaPacks. La UI, la billetera y el guardado no cambian.
public interface IPurchaseBackend
{
    // La tienda real tarda en conectarse al arrancar: hasta entonces no hay nada que vender.
    bool Listo { get; }

    void Comprar(PackDefinition pack, Action<ResultadoCompra> alTerminar);
}
