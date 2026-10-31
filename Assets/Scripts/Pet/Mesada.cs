using System.Collections.Generic;
using UnityEngine;

/// La mesada de la cocina: pone en sus lugares los platos que hay en el inventario. Si hay mas
/// platos que lugares, las flechas los corren de a uno, como un carrusel.
public class Mesada : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlatoEnMesada[] lugares;
    [SerializeField] private TouchTarget flechaIzquierda;
    [SerializeField] private TouchTarget flechaDerecha;

    [Header("Recetas")]
    // Todas las que se pueden cocinar, en el orden en que aparecen en la mesada.
    [SerializeField] private RecipeDefinition[] recetas;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoFlecha;

    private readonly List<RecipeDefinition> hay = new List<RecipeDefinition>();
    private int primero;
    private bool empezada;

    private void OnEnable()
    {
        Despensa.Platos.Cambio += AlCambiar;
        flechaIzquierda.Tocado += Anterior;
        flechaDerecha.Tocado += Siguiente;
        // La primera vez espera a Start: los platos todavia pueden no haber corrido su Awake.
        if (empezada) Actualizar();
    }

    private void Start()
    {
        empezada = true;
        Actualizar();
    }

    private void OnDisable()
    {
        Despensa.Platos.Cambio -= AlCambiar;
        flechaIzquierda.Tocado -= Anterior;
        flechaDerecha.Tocado -= Siguiente;
    }

    private void AlCambiar(string id, int cantidad)
    {
        Actualizar();
    }

    private void Anterior()
    {
        primero--;
        SoundPlayer.Reproducir(sonidoFlecha);
        Actualizar();
    }

    private void Siguiente()
    {
        primero++;
        SoundPlayer.Reproducir(sonidoFlecha);
        Actualizar();
    }

    private void Actualizar()
    {
        // Se rearma solo cuando cambia el inventario o se toca una flecha, no en cada frame.
        hay.Clear();
        foreach (RecipeDefinition receta in recetas)
        {
            if (Despensa.Platos.Cantidad(receta.Id) > 0) hay.Add(receta);
        }

        // Si se termino un plato, la lista se achica: el carrusel no puede quedar mirando al vacio.
        primero = Mathf.Clamp(primero, 0, Mathf.Max(0, hay.Count - lugares.Length));

        for (int i = 0; i < lugares.Length; i++)
        {
            int indice = primero + i;
            lugares[i].Mostrar(indice < hay.Count ? hay[indice] : null);
        }

        flechaIzquierda.gameObject.SetActive(primero > 0);
        flechaDerecha.gameObject.SetActive(primero + lugares.Length < hay.Count);
    }
}
