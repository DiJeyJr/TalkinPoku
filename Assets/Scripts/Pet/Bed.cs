using UnityEngine;

/// Cama del dormitorio: un tap acuesta o despierta a la mascota. Mientras duerme, NeedsSystem
/// recupera energia en vez de gastarla, y la despierta solo al llegar a 100.
[RequireComponent(typeof(TouchTarget))]
public class Bed : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoDormir;

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

    private void AlTocar()
    {
        if (necesidades == null) return;
        necesidades.Dormir(!necesidades.Dormido);
        // Solo al acostarlo: despertarlo no lleva sonido de cuna.
        if (necesidades.Dormido) SoundPlayer.Reproducir(sonidoDormir);
    }
}
