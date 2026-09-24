using UnityEngine;

/// Oscurece la pantalla mientras la mascota duerme. Va en el Canvas y no en la cama porque la cama
/// se apaga al cambiar de habitacion, y la mascota se puede despertar sola estando en otra.
public class SleepOverlay : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    [SerializeField] private GameObject overlay;

    private void OnEnable()
    {
        if (necesidades == null) return;
        necesidades.DormidoCambiado += Mostrar;
        Mostrar(necesidades.Dormido);
    }

    private void OnDisable()
    {
        if (necesidades != null) necesidades.DormidoCambiado -= Mostrar;
    }

    private void Mostrar(bool dormido)
    {
        if (overlay != null) overlay.SetActive(dormido);
    }
}
