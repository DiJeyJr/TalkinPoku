using UnityEngine;

// ============================================================================
// Latido.cs
// QUE HACE: hace "latir" un elemento de la UI: crece de golpe y vuelve a su
//           tamano. Otros scripts llaman a Latir().
// DONDE ESTA: en las barras de necesidades, el contador de monedas, el boton
//             Mega, el cartel de nivel y los productos de la tienda.
// PARA CAMBIAR: cuanto crece -> escalaMaxima. Cuanto dura -> duracion.
// ============================================================================

// Hace latir un elemento: crece de golpe y vuelve a su tamano. Lo usan las barras cuando suben de
// golpe y el contador de monedas cuando entra una.
public class Latido : MonoBehaviour
{
    [Header("Feedback")]
    // [AJUSTE] Escala maxima del latido (1.2 = 20% mas grande).
    [SerializeField] private float escalaMaxima = 1.2f;
    // [AJUSTE] Segundos que tarda en volver a su tamano.
    [SerializeField] private float duracion = 0.25f;

    private Vector3 escalaBase;
    // Cuanto extra esta agrandado ahora (baja hasta 0).
    private float extra;

    private void Awake()
    {
        escalaBase = transform.localScale;
    }

    // Si ya estaba latiendo vuelve a crecer desde donde esta, sin achicarse primero: con varias
    // monedas seguidas el contador queda inflado hasta que entra la ultima.
    public void Latir()
    {
        extra = escalaMaxima - 1f;
    }

    // Achica de a poco hasta el tamano normal.
    private void Update()
    {
        if (extra <= 0f) return;
        extra = Mathf.MoveTowards(extra, 0f, (escalaMaxima - 1f) / duracion * Time.unscaledDeltaTime);
        transform.localScale = escalaBase * (1f + extra);
    }
}
