using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Una fila del libro de recetas: el plato, el nombre y los ingredientes.
public class FilaReceta : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image plato;
    [SerializeField] private TMP_Text nombre;
    [SerializeField] private Image[] ingredientes;
    [SerializeField] private Sprite spriteOculto;

    [Header("Colores")]
    [SerializeField] private Color colorOculto = new Color(0.25f, 0.2f, 0.3f, 0.6f);

    public void Mostrar(RecipeDefinition receta, bool descubierta)
    {
        plato.sprite = receta.Sprite;
        // Sin descubrir se ve la silueta: dice que el plato existe sin decir cual es.
        plato.color = descubierta ? Color.white : colorOculto;
        nombre.SetText(descubierta ? receta.Nombre : "???");

        for (int i = 0; i < ingredientes.Length; i++)
        {
            bool lleva = i < receta.Ingredientes.Count;
            ingredientes[i].enabled = lleva;
            if (!lleva) continue;

            // Antes de descubrirla se ve cuantos ingredientes lleva, pero no cuales.
            ingredientes[i].sprite = descubierta ? receta.Ingredientes[i].Sprite : spriteOculto;
            ingredientes[i].color = descubierta ? Color.white : colorOculto;
        }
    }
}
