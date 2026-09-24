using UnityEngine;

/// Jugar con la mascota tocandola: un tap le da un poco de diversion y mantener el dedo apoyado
/// (una caricia) le da diversion mientras dure.
[RequireComponent(typeof(TouchTarget))]
public class PetInteraction : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;

    [Header("Diversion")]
    [SerializeField] private float porToque = 3f;
    [SerializeField] private float porSegundoAcariciando = 5f;

    private TouchTarget toque;

    private void Awake()
    {
        toque = GetComponent<TouchTarget>();
    }

    private void OnEnable()
    {
        toque.Tocado += AlTocar;
    }

    private void OnDisable()
    {
        toque.Tocado -= AlTocar;
    }

    private void Update()
    {
        if (toque.Sosteniendo && !necesidades.Dormido)
        {
            necesidades.Sumar(NeedType.Diversion, porSegundoAcariciando * Time.deltaTime);
        }
    }

    private void AlTocar()
    {
        // Dormida no juega: tocarla no la entretiene hasta que se despierte.
        if (!necesidades.Dormido) necesidades.Sumar(NeedType.Diversion, porToque);
    }
}
