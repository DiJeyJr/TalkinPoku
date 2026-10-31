using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Cartel de "¿Seguro?" con Aceptar y Cancelar. El GDD pide confirmar antes de gastar: el publico es
/// infantil y un toque sin querer no puede llevarse las monedas.
public class ConfirmDialog : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text texto;
    [SerializeField] private Button aceptar;
    [SerializeField] private Button cancelar;
    // La caja comun: la usan Avisar y Preguntar.
    [SerializeField] private GameObject caja;

    [Header("Compra")]
    // El cartel de compra que dibujo Martina: titulo y botones vienen en el dibujo, aca se llena
    // el producto. Los botones son transparentes, encima de los dibujados.
    [SerializeField] private GameObject cajaCompra;
    [SerializeField] private Image compraIcono;
    [SerializeField] private TMP_Text compraNombre;
    [SerializeField] private TMP_Text compraPrecio;
    [SerializeField] private Image compraMoneda;
    [SerializeField] private Button comprar;
    [SerializeField] private Button noComprar;

    private Action alAceptar;

    private void Awake()
    {
        aceptar.onClick.AddListener(Aceptar);
        cancelar.onClick.AddListener(Cerrar);
        comprar.onClick.AddListener(Aceptar);
        noComprar.onClick.AddListener(Cerrar);
        panel.SetActive(false);
    }

    private void OnDisable()
    {
        // Si se cierra la tienda con el cartel abierto, al volver no tiene que seguir ahi.
        Cerrar();
    }

    public void Preguntar(string mensaje, Action siAcepta)
    {
        texto.SetText(mensaje);
        alAceptar = siAcepta;
        cancelar.gameObject.SetActive(true);
        Mostrar(caja);
    }

    public void PreguntarCompra(ItemDefinition item, Sprite moneda, Action siAcepta)
    {
        compraIcono.sprite = item.Icono;
        compraIcono.preserveAspect = true;
        compraNombre.SetText(item.Nombre);
        compraPrecio.SetText("{0}", item.Precio);
        compraMoneda.sprite = moneda;
        alAceptar = siAcepta;
        Mostrar(cajaCompra);
    }

    // Solo para avisar (no alcanza la plata): un boton y nada que hacer al aceptar.
    public void Avisar(string mensaje)
    {
        texto.SetText(mensaje);
        alAceptar = null;
        cancelar.gameObject.SetActive(false);
        Mostrar(caja);
    }

    private void Mostrar(GameObject cual)
    {
        caja.SetActive(cual == caja);
        cajaCompra.SetActive(cual == cajaCompra);
        panel.SetActive(true);
    }

    private void Aceptar()
    {
        Action accion = alAceptar;
        Cerrar();
        accion?.Invoke();
    }

    private void Cerrar()
    {
        alAceptar = null;
        if (panel != null) panel.SetActive(false);
    }
}
