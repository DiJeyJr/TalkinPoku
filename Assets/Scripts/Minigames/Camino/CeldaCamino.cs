using System;
using UnityEngine;

// ============================================================================
// CeldaCamino.cs  (minijuego "Camino a casa")
// QUE HACE: una celda de la grilla. Se pinta segun su estado (normal,
//           resaltada, acertada, error) y avisa cuando la tocan.
// DONDE ESTA: es el prefab CeldaCamino; GrillaCamino crea una por casillero.
// PARA CAMBIAR: los colores de cada estado -> los campos de Colores del prefab.
// ============================================================================

// Una celda de la grilla. Solo sabe pintarse segun su estado y avisar que la tocaron; si el toque
// estuvo bien o mal lo decide CaminoGame.
[RequireComponent(typeof(SpriteRenderer), typeof(TouchTarget))]
public class CeldaCamino : MonoBehaviour
{
    public enum Estado { Normal, Resaltada, Acertada, Error }

    [Header("Colores")]
    // [AJUSTE] Color de cada estado.
    [SerializeField] private Color colorNormal = new Color(0.85f, 0.85f, 0.85f);
    [SerializeField] private Color colorResaltada = new Color(1f, 0.85f, 0.3f);
    [SerializeField] private Color colorAcertada = new Color(0.45f, 0.85f, 0.45f);
    [SerializeField] private Color colorError = new Color(0.95f, 0.35f, 0.35f);

    public event Action<CeldaCamino> Tocada;

    // Columna y fila dentro de la grilla. El camino se guarda con esto y no con la referencia,
    // asi generarlo no depende de que existan los objetos.
    public Vector2Int Posicion { get; private set; }

    private SpriteRenderer sprite;
    private TouchTarget touchTarget;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        touchTarget = GetComponent<TouchTarget>();
    }

    private void OnEnable() => touchTarget.Tocado += AvisarToque;
    private void OnDisable() => touchTarget.Tocado -= AvisarToque;

    // La llama la grilla al crearla.
    public void Inicializar(Vector2Int posicion)
    {
        Posicion = posicion;
        CambiarEstado(Estado.Normal);
    }

    // Cambia el color segun el estado.
    public void CambiarEstado(Estado estado)
    {
        switch (estado)
        {
            case Estado.Resaltada: sprite.color = colorResaltada; break;
            case Estado.Acertada: sprite.color = colorAcertada; break;
            case Estado.Error: sprite.color = colorError; break;
            default: sprite.color = colorNormal; break;
        }
    }

    private void AvisarToque() => Tocada?.Invoke(this);
}
