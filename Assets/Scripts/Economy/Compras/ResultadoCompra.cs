/// Como termino una compra con dinero real. Cancelado no es un error: el adulto cerro la ventana de
/// pago, y no hay que mostrarle un cartel de falla.
public enum ResultadoCompra
{
    Exito,
    Fallo,
    Cancelado
}
