using System;

// Quien cobra de verdad. La tienda de packs solo conoce esta interfaz: hoy la implementa
// BackendSimulado, y para cobrar en serio alcanza con otra clase que use Unity IAP.
public interface IPurchaseBackend
{
    // La tienda real tarda en conectarse al arrancar: hasta entonces no hay nada que vender.
    bool Listo { get; }

    void Comprar(PackDefinition pack, Action<ResultadoCompra> alTerminar);
}
