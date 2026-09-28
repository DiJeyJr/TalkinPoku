using System;
using UnityEngine;

/// Una mancha sobre Poku. Se va borrando mientras la esponja pasa por encima: cuenta la distancia
/// frotada y no el tiempo, asi dejar la esponja quieta no limpia.
[RequireComponent(typeof(SpriteRenderer))]
public class Stain : MonoBehaviour
{
    [Header("Umbrales")]
    // Cuantas unidades de mundo hay que frotar encima para sacarla entera.
    [SerializeField] private float frotadoParaLimpiar = 2f;

    [Header("Feedback")]
    [SerializeField] private float opacidadMinima = 0.25f;

    public event Action Limpiada;

    private SpriteRenderer spriteRenderer;
    private float suciedad;

    public bool Activa => gameObject.activeSelf;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Ensuciar()
    {
        // Primero se prende: si nunca estuvo activa, recien ahi corre Awake y existe el SpriteRenderer.
        gameObject.SetActive(true);
        suciedad = 1f;
        Pintar();
    }

    public void Quitar()
    {
        gameObject.SetActive(false);
    }

    public bool Toca(Vector2 centro, float alcance)
    {
        // Alcanza con que la esponja la roce: se suman el radio de la mancha y el de la esponja.
        float radio = spriteRenderer.bounds.extents.x;
        return Vector2.Distance(transform.position, centro) <= radio + alcance;
    }

    public void Frotar(float distancia)
    {
        suciedad -= distancia / frotadoParaLimpiar;
        if (suciedad > 0f)
        {
            Pintar();
            return;
        }

        Quitar();
        Limpiada?.Invoke();
    }

    private void Pintar()
    {
        // No llega a cero mientras falte frotar: si se volviera invisible, el jugador creeria que ya salio.
        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(opacidadMinima, 1f, suciedad);
        spriteRenderer.color = color;
    }
}
