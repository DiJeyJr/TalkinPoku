using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// El libro de recetas: una fila por receta, con el plato y sus ingredientes si ya se descubrio, o
/// la silueta y signos de pregunta si no. Mientras esta abierto se apaga la cocina de atras, porque
/// el arrastre de los ingredientes lee el touch directo y no se entera de que hay UI encima.
public class LibroDeRecetas : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform contenedorFilas;
    [SerializeField] private FilaReceta filaPrefab;
    [SerializeField] private TMP_Text textoProgreso;
    [SerializeField] private Button botonAbrir;
    [SerializeField] private Button botonCerrar;
    [SerializeField] private GameObject cocina;

    private readonly List<FilaReceta> filas = new List<FilaReceta>();

    private void Awake()
    {
        botonAbrir.onClick.AddListener(Abrir);
        botonCerrar.onClick.AddListener(Cerrar);
        panel.SetActive(false);
    }

    public void Mostrar(IReadOnlyList<RecipeDefinition> recetas)
    {
        // Las filas se crean la primera vez; despues solo se repintan.
        while (filas.Count < recetas.Count) filas.Add(Instantiate(filaPrefab, contenedorFilas));

        int descubiertas = 0;
        for (int i = 0; i < recetas.Count; i++)
        {
            bool conocida = RecipeBook.EstaDescubierta(recetas[i].Id);
            if (conocida) descubiertas++;
            filas[i].Mostrar(recetas[i], conocida);
        }

        textoProgreso.SetText("{0} de {1}", descubiertas, recetas.Count);
    }

    private void Abrir()
    {
        panel.SetActive(true);
        cocina.SetActive(false);
    }

    private void Cerrar()
    {
        panel.SetActive(false);
        cocina.SetActive(true);
    }
}
