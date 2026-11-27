using System;
using UnityEngine;

// ============================================================================
// Stain.cs
// QUE HACE: una mancha sobre Poku. Se va borrando (se vuelve transparente) a
//           medida que la esponja pasa por encima, y cuando se borra del todo
//           avisa con el evento Limpiada.
// DONDE ESTA: en cada hijo de "Poku/Manchas" (Cachete, Cola, Pecho, etc).
// PARA CAMBIAR: cuanto hay que frotar para sacarla -> frotadoParaLimpiar.
//               que tan transparente queda antes de salir -> opacidadMinima.
// ============================================================================

// Una mancha sobre Poku. Se va borrando mientras la esponja pasa por encima: cuenta la distancia
// frotada y no el tiempo, asi dejar la esponja quieta no limpia.
[RequireComponent(typeof(SpriteRenderer))]
public class Stain : MonoBehaviour
{
    [Header("Umbrales")]
    // [AJUSTE] Distancia a frotar para limpiarla.
    // Cuantas unidades de mundo hay que frotar encima para sacarla entera.
    [SerializeField] private float frotadoParaLimpiar = 2f;

    [Header("Feedback")]
    // [AJUSTE] Opacidad minima mientras se limpia.
    [SerializeField] private float opacidadMinima = 0.25f;

    public event Action Limpiada;

    private SpriteRenderer spriteRenderer;
    // 1 = sucia del todo, 0 = limpia.
    private float suciedad;

    public bool Activa => gameObject.activeSelf;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Aparece la mancha, sucia del todo.
    public void Ensuciar()
    {
        // Primero se prende: si nunca estuvo activa, recien ahi corre Awake y existe el SpriteRenderer.
        gameObject.SetActive(true);
        suciedad = 1f;
        Pintar();
    }

    // Desaparece la mancha (sin avisar).
    public void Quitar()
    {
        gameObject.SetActive(false);
    }

    // true si la esponja (centro + alcance) esta encima de esta mancha.
    public bool Toca(Vector2 centro, float alcance)
    {
        // Alcanza con que la esponja la roce: se suman el radio de la mancha y el de la esponja.
        float radio = spriteRenderer.bounds.extents.x;
        return Vector2.Distance(transform.position, centro) <= radio + alcance;
    }

    // Le resta suciedad segun la distancia frotada. Si llega a 0, desaparece y avisa.
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

    // Ajusta la transparencia segun cuanta suciedad queda.
    private void Pintar()
    {
        // No llega a cero mientras falte frotar: si se volviera invisible, el jugador creeria que ya salio.
        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(opacidadMinima, 1f, suciedad);
        spriteRenderer.color = color;
    }
}
