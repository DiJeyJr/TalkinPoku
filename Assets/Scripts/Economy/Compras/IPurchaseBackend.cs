using System;

// ============================================================================
// IPurchaseBackend.cs
// QUE HACE: interfaz (contrato) de "quien cobra" las compras con dinero real.
//           La tienda de packs solo conoce esta interfaz, asi se puede cambiar
//           la tienda simulada por una real sin tocar la tienda.
// IMPLEMENTACION ACTUAL: BackendSimulado.
// ============================================================================

// Quien cobra de verdad. La tienda de packs solo conoce esta interfaz: hoy la implementa
// BackendSimulado, y para cobrar en serio alcanza con otra clase que use Unity IAP.
public interface IPurchaseBackend
{
    // La tienda real tarda en conectarse al arrancar: hasta entonces no hay nada que vender.
    bool Listo { get; }

    // Compra un pack y, cuando termina, llama a alTerminar con el resultado.
    void Comprar(PackDefinition pack, Action<ResultadoCompra> alTerminar);
}
