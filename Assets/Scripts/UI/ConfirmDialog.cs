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

    private Action alAceptar;

    private void Awake()
    {
        aceptar.onClick.AddListener(Aceptar);
        cancelar.onClick.AddListener(Cerrar);
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
        panel.SetActive(true);
    }

    // Solo para avisar (no alcanza la plata): un boton y nada que hacer al aceptar.
    public void Avisar(string mensaje)
    {
        texto.SetText(mensaje);
        alAceptar = null;
        cancelar.gameObject.SetActive(false);
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
