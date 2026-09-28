using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Muestra en que habitacion esta el jugador: el nombre arriba y abajo un icono por habitacion
/// (un puntito si todavia no tiene icono). Tocar uno lleva a esa habitacion.
/// Se entera por el evento del RoomNavigator, asi no revisa nada en Update.
public class RoomIndicator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RoomNavigator navegador;
    [SerializeField] private TMP_Text nombre;
    [SerializeField] private Transform contenedorPuntos;
    [SerializeField] private Image puntoPrefab;

    [Header("Iconos")]
    // En el mismo orden que las habitaciones del navegador. Un lugar vacio deja el puntito.
    [SerializeField] private Sprite[] iconos;
    [SerializeField] private float ladoIcono = 90f;
    [SerializeField] private float escalaActiva = 1.25f;

    [Header("Colores")]
    [SerializeField] private Color colorActivo = Color.white;
    [SerializeField] private Color colorInactivo = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] private Color colorIconoInactivo = new Color(1f, 1f, 1f, 0.5f);

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

        // Uno por habitacion: si se agrega una al array del navegador, aparece sola.
        if (puntoPrefab != null && contenedorPuntos != null)
        {
            for (int i = 0; i < navegador.Cantidad; i++)
            {
                Image punto = Instantiate(puntoPrefab, contenedorPuntos);
                if (TieneIcono(i))
                {
                    punto.sprite = iconos[i];
                    punto.preserveAspect = true;
                    punto.rectTransform.sizeDelta = new Vector2(ladoIcono, ladoIcono);
                }

                // El prefab viene sin raycast porque antes solo mostraba; ahora se toca. El Button ya
                // viene en el prefab, con el BotonFeedback (pop y achique) que tienen todos los botones.
                punto.raycastTarget = true;
                Button boton = punto.GetComponent<Button>();
                boton.targetGraphic = punto;

                // Copia local: si la lambda usara i, todos los botones verian el valor final del for.
                int indice = i;
                boton.onClick.AddListener(() => navegador.IrA(indice));
                puntos.Add(punto);
            }
        }

        // El Start del navegador puede correr antes que este y avisar cuando todavia no habia
        // puntitos (Unity no garantiza el orden entre objetos), asi que se pinta a mano.
        Actualizar(navegador.Actual);
    }

    private bool TieneIcono(int i)
    {
        return iconos != null && i < iconos.Length && iconos[i] != null;
    }

    private void Actualizar(int indice)
    {
        if (nombre != null) nombre.SetText(navegador.NombreActual);

        for (int i = 0; i < puntos.Count; i++)
        {
            bool activo = i == indice;
            if (TieneIcono(i))
            {
                puntos[i].color = activo ? Color.white : colorIconoInactivo;
                // Se agranda con la escala y no con el tamanio: el layout no mira la escala,
                // asi el activo crece sin correr a los demas.
                puntos[i].rectTransform.localScale = Vector3.one * (activo ? escalaActiva : 1f);
            }
            else
            {
                puntos[i].color = activo ? colorActivo : colorInactivo;
            }
        }
    }
}
