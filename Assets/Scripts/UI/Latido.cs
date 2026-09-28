using UnityEngine;

/// Hace latir un elemento: crece de golpe y vuelve a su tamano. Lo usan las barras cuando suben de
/// golpe y el contador de monedas cuando entra una.
public class Latido : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] private float escalaMaxima = 1.2f;
    [SerializeField] private float duracion = 0.25f;

    private Vector3 escalaBase;
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

    private void Update()
    {
        if (extra <= 0f) return;
        extra = Mathf.MoveTowards(extra, 0f, (escalaMaxima - 1f) / duracion * Time.unscaledDeltaTime);
        transform.localScale = escalaBase * (1f + extra);
    }
}
