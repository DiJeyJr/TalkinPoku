using UnityEngine;

// ============================================================================
// PetInteraction.cs
// QUE HACE: jugar con Poku tocandolo. Un tap le da un poco de diversion y lo
//           aplasta; mantener el dedo apoyado (caricia) le da diversion por
//           segundo y experiencia.
// DONDE ESTA: en el objeto "Poku" de SampleScene (usa TouchTarget).
// PARA CAMBIAR: diversion por tap -> porToque.
//               diversion por segundo acariciando -> porSegundoAcariciando.
//               XP de la caricia -> ProgressionConfig.asset (fuente "Acariciar").
// ============================================================================

// Jugar con la mascota tocandola: un tap le da un poco de diversion y mantener el dedo apoyado
// (una caricia) le da diversion mientras dure.
[RequireComponent(typeof(TouchTarget))]
public class PetInteraction : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    [SerializeField] private Progression progreso;
    [SerializeField] private PetJuice juice;

    [Header("Diversion")]
    // [AJUSTE] Diversion que da cada tap.
    [SerializeField] private float porToque = 3f;
    // [AJUSTE] Diversion por segundo mientras se lo acaricia.
    [SerializeField] private float porSegundoAcariciando = 5f;

    [Header("Feedback")]
    // La caricia arranca de un dedo apoyado, no de un golpe: el aplastado va mas suave que en el tap.
    [SerializeField] private float intensidadCaricia = 0.6f;

    private TouchTarget toque;

    private void Awake()
    {
        toque = GetComponent<TouchTarget>();
    }

    private void OnEnable()
    {
        toque.Tocado += AlTocar;
        toque.HoldEmpezado += AlEmpezarCaricia;
    }

    private void OnDisable()
    {
        toque.Tocado -= AlTocar;
        toque.HoldEmpezado -= AlEmpezarCaricia;
    }

    // Mientras el dedo siga apoyado, suma diversion de a poco (por segundo * deltaTime).
    private void Update()
    {
        if (toque.Sosteniendo && !necesidades.Dormido)
        {
            necesidades.Sumar(NeedType.Diversion, porSegundoAcariciando * Time.deltaTime);
        }
    }

    // Tap: diversion y animacion de aplastarse.
    private void AlTocar()
    {
        // Dormida no juega: tocarla no la entretiene hasta que se despierte.
        if (necesidades.Dormido) return;

        necesidades.Sumar(NeedType.Diversion, porToque);
        if (juice != null) juice.Aplastar();
    }

    // Empieza la caricia: XP, aplastado suave y vibracion.
    private void AlEmpezarCaricia()
    {
        // La experiencia va una vez por caricia y no por segundo: dejar el dedo apoyado no sube de nivel.
        if (necesidades.Dormido) return;

        if (progreso != null) progreso.Ganar(XpSource.Acariciar);
        if (juice != null) juice.Aplastar(intensidadCaricia);
        Vibracion.Vibrar();
    }
}
