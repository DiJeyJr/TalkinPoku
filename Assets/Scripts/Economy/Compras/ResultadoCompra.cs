// ============================================================================
// ResultadoCompra.cs
// QUE HACE: los resultados posibles de una compra con dinero real.
// ============================================================================
public enum ResultadoCompra
{
    Exito,      // se cobro: se entrega el pack
    Fallo,      // algo salio mal: se muestra un cartel de error
    // El adulto cerro la ventana de pago: no es un error y no se muestra cartel de falla.
    Cancelado
}
