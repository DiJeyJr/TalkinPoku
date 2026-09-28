using System;
using System.Collections.Generic;
using UnityEngine;

/// La olla: recibe los ingredientes que se sueltan encima, muestra cuales tiene y se vacia al
/// tocarla. No sabe de recetas; que sale lo decide CocinaGame.
[RequireComponent(typeof(Collider2D))]
public class Olla : MonoBehaviour
{
    [Header("Referencias")]
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

    private void OnEnable()
    {
        if (toque != null) toque.Tocado += Vaciar;
    }

    private void OnDisable()
    {
        if (toque != null) toque.Tocado -= Vaciar;
    }

    public bool Contiene(Vector2 puntoMundo)
    {
        return miCollider.OverlapPoint(puntoMundo);
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

    public void Vaciar()
    {
        if (contenido.Count == 0) return;

        contenido.Clear();
        Mostrar();
        ContenidoCambiado?.Invoke();
    }

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
