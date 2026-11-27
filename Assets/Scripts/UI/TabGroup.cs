using UnityEngine;
using UnityEngine.UI;

// ============================================================================
// TabGroup.cs
// QUE HACE: las pestanas de la tienda. Muestra un panel y apaga los demas.
// DONDE ESTA: en "Canvas/Pantallas/Tienda" de SampleScene.
// PARA CAMBIAR: agregar una pestana -> sumar su boton a "Pestanas" y su panel a
//               "Paneles" (mismo indice), y en el OnClick del boton llamar a
//               Mostrar con ese indice.
//               colores de pestana activa/inactiva -> colorActiva / colorInactiva.
// ============================================================================

// Pestanas de la tienda: muestra un panel y apaga los demas. Cada boton de pestana llama a
// Mostrar con su numero desde el OnClick del Inspector.
public class TabGroup : MonoBehaviour
{
    [Header("Referencias")]
    // Mismo orden en las dos listas: la pestana 0 abre el panel 0.
    [SerializeField] private Image[] pestanas;
    [SerializeField] private GameObject[] paneles;

    [Header("Colores")]
    [SerializeField] private Color colorActiva = Color.white;
    [SerializeField] private Color colorInactiva = new Color(1f, 1f, 1f, 0.5f);

    // Cada vez que se abre la tienda arranca en la primera pestana, no en la que quedo la vez anterior.
    private void OnEnable() => Mostrar(0);

    // Prende el panel "indice", apaga los demas y pinta las pestanas.
    public void Mostrar(int indice)
    {
        for (int i = 0; i < paneles.Length; i++)
        {
            paneles[i].SetActive(i == indice);
        }

        for (int i = 0; i < pestanas.Length; i++)
        {
            pestanas[i].color = i == indice ? colorActiva : colorInactiva;
        }
    }
}
