using System.Collections.Generic;
using UnityEngine;

// ============================================================================
// Mesada.cs
// QUE HACE: la mesada de la cocina de la casa. Muestra los platos cocinados que
//           tiene el jugador (de la Despensa de platos). Si hay mas platos que
//           lugares, las flechas los van corriendo como un carrusel.
// DONDE ESTA: en "Habitaciones/Cocina/Mesada" de SampleScene. Cada lugar es un
//             PlatoEnMesada (Plato1, Plato2, Plato3).
// PARA CAMBIAR: que platos pueden aparecer y en que orden -> lista "Recetas"
//               del Inspector. Cuantos lugares hay -> lista "Lugares".
// ============================================================================

// La mesada de la cocina: pone en sus lugares los platos que hay en el inventario. Si hay mas
// platos que lugares, las flechas los corren de a uno, como un carrusel.
public class Mesada : MonoBehaviour
{
    [Header("Referencias")]
    // [AJUSTE] Los lugares donde se muestra un plato.
    [SerializeField] private PlatoEnMesada[] lugares;
    [SerializeField] private TouchTarget flechaIzquierda;
    [SerializeField] private TouchTarget flechaDerecha;

    [Header("Recetas")]
    // [AJUSTE] Recetas que se muestran.
    // Todas las que se pueden cocinar, en el orden en que aparecen en la mesada.
    [SerializeField] private RecipeDefinition[] recetas;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoFlecha;

    // Recetas de las que hay al menos un plato.
    private readonly List<RecipeDefinition> hay = new List<RecipeDefinition>();
    // Indice del primer plato visible en el carrusel.
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

    // Cambio la cantidad de algun plato (se cocino o se comio uno).
    private void AlCambiar(string id, int cantidad)
    {
        Actualizar();
    }

    // Flecha izquierda: corre el carrusel uno para atras.
    private void Anterior()
    {
        primero--;
        SoundPlayer.Reproducir(sonidoFlecha);
        Actualizar();
    }

    // Flecha derecha: corre el carrusel uno para adelante.
    private void Siguiente()
    {
        primero++;
        SoundPlayer.Reproducir(sonidoFlecha);
        Actualizar();
    }

    // Arma la lista de platos disponibles, llena los lugares y prende/apaga las flechas.
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

        // Las flechas solo aparecen si hay platos para ese lado.
        flechaIzquierda.gameObject.SetActive(primero > 0);
        flechaDerecha.gameObject.SetActive(primero + lugares.Length < hay.Count);
    }
}
