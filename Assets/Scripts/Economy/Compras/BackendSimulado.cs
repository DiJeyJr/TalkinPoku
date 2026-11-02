using System;
using System.Collections;
using UnityEngine;

/// Hace de tienda sin cobrar nada: espera como si hablara con Google Play y contesta lo que diga el
/// Inspector. Sin ficha publicada en Play Console no hay productos reales para probar.
public class BackendSimulado : MonoBehaviour, IPurchaseBackend
{
    [Header("Simulacion")]
    [SerializeField] private float demora = 1.5f;
    // Con Fallo o Cancelado se prueba como responde la tienda cuando algo sale mal.
    [SerializeField] private ResultadoCompra respuesta = ResultadoCompra.Exito;

    public bool Listo => true;

    private Action<ResultadoCompra> pendiente;

    public void Comprar(PackDefinition pack, Action<ResultadoCompra> alTerminar)
    {
        // Apagado no puede esperar: se contesta ya, si no la tienda queda trabada en "procesando".
        if (!isActiveAndEnabled)
        {
            alTerminar?.Invoke(ResultadoCompra.Cancelado);
            return;
        }
        pendiente = alTerminar;
        StartCoroutine(Responder(pack));
    }

    private void OnDisable()
    {
        // Al apagarse muere la corrutina: la compra en curso se da por cancelada, sin cobrar.
        Action<ResultadoCompra> alTerminar = pendiente;
        pendiente = null;
        alTerminar?.Invoke(ResultadoCompra.Cancelado);
    }

    private IEnumerator Responder(PackDefinition pack)
    {
        // Realtime: la que tarda es la tienda, no el juego. Con timeScale en 0 tambien tiene que contestar.
        yield return new WaitForSecondsRealtime(demora);
        Debug.Log("Compra simulada de " + pack.ProductId + ": " + respuesta);
        Action<ResultadoCompra> alTerminar = pendiente;
        pendiente = null;
        alTerminar?.Invoke(respuesta);
    }
}
