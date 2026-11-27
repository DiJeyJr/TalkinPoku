using System;
using System.Collections.Generic;
using UnityEngine;

// ============================================================================
// Olla.cs  (minijuego "Cocina")
// QUE HACE: la olla. Recibe los ingredientes que se sueltan encima (uno de cada
//           tipo), los muestra, y se vacia al tocarla. No sabe de recetas.
// DONDE ESTA: en "Cocina/Olla" de la escena Cocina.
// PARA CAMBIAR: cuantos ingredientes entran -> cantidad de elementos de la
//               lista "Lugares" (un SpriteRenderer por lugar).
// AVISA: ContenidoCambiado cuando entra o sale algo.
// ============================================================================

// La olla: recibe los ingredientes que se sueltan encima, muestra cuales tiene y se vacia al
// tocarla. No sabe de recetas; que sale lo decide CocinaGame.
[RequireComponent(typeof(Collider2D))]
public class Olla : MonoBehaviour
{
    [Header("Referencias")]
    // [AJUSTE] Capacidad de la olla.
    // Uno por ingrediente que entra: la cantidad de lugares es la capacidad de la olla.
    [SerializeField] private SpriteRenderer[] lugares;
    [SerializeField] private TouchTarget toque;

    public event Action ContenidoCambiado;

    public IReadOnlyList<IngredientDefinition> Contenido => contenido;

    private readonly List<IngredientDefinition> contenido = new List<IngredientDefinition>();
    private Collider2D miCollider;

    private void Awake()
    {
        miCollider = GetComponent<Collider2D>();
        Mostrar();
    }

    // Un tap en la olla la vacia.
    private void OnEnable()
    {
        if (toque != null) toque.Tocado += Vaciar;
    }

    private void OnDisable()
    {
        if (toque != null) toque.Tocado -= Vaciar;
    }

    // true si un punto cae sobre la olla.
    public bool Contiene(Vector2 puntoMundo)
    {
        return miCollider.OverlapPoint(puntoMundo);
    }

    public bool Tiene(IngredientDefinition ingrediente)
    {
        return contenido.Contains(ingrediente);
    }

    // false si ya estaba o si no hay lugar. Repetido no suma: las recetas son de ingredientes distintos.
    public bool Agregar(IngredientDefinition ingrediente)
    {
        if (contenido.Count >= lugares.Length || contenido.Contains(ingrediente)) return false;

        contenido.Add(ingrediente);
        Mostrar();
        ContenidoCambiado?.Invoke();
        return true;
    }

    // Saca todo de la olla.
    public void Vaciar()
    {
        if (contenido.Count == 0) return;

        contenido.Clear();
        Mostrar();
        ContenidoCambiado?.Invoke();
    }

    // Prende un lugar por cada ingrediente que hay adentro.
    private void Mostrar()
    {
        for (int i = 0; i < lugares.Length; i++)
        {
            bool ocupado = i < contenido.Count;
            lugares[i].enabled = ocupado;
            if (ocupado) lugares[i].sprite = contenido[i].Sprite;
        }
    }
}
