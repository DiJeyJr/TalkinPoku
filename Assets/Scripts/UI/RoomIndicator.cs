using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Muestra en que habitacion esta el jugador: el nombre arriba y un puntito por habitacion.
/// Se entera por el evento del RoomNavigator, asi no revisa nada en Update.
public class RoomIndicator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RoomNavigator navegador;
    [SerializeField] private TMP_Text nombre;
    [SerializeField] private Transform contenedorPuntos;
    [SerializeField] private Image puntoPrefab;

    [Header("Colores")]
    [SerializeField] private Color colorActivo = Color.white;
    [SerializeField] private Color colorInactivo = new Color(1f, 1f, 1f, 0.35f);

    private readonly List<Image> puntos = new List<Image>();

    private void OnEnable()
    {
        if (navegador != null) navegador.HabitacionCambiada += Actualizar;
    }

    private void OnDisable()
    {
        if (navegador != null) navegador.HabitacionCambiada -= Actualizar;
    }

    private void Start()
    {
        if (navegador == null) return;

        // Un puntito por habitacion: si se agrega una al array del navegador, aparece sola.
        if (puntoPrefab != null && contenedorPuntos != null)
        {
            for (int i = 0; i < navegador.Cantidad; i++)
            {
                puntos.Add(Instantiate(puntoPrefab, contenedorPuntos));
            }
        }

        // El Start del navegador puede correr antes que este y avisar cuando todavia no habia
        // puntitos (Unity no garantiza el orden entre objetos), asi que se pinta a mano.
        Actualizar(navegador.Actual);
    }

    private void Actualizar(int indice)
    {
        if (nombre != null) nombre.SetText(navegador.NombreActual);

        for (int i = 0; i < puntos.Count; i++)
        {
            puntos[i].color = i == indice ? colorActivo : colorInactivo;
        }
    }
}
