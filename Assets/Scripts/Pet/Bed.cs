using UnityEngine;

// ============================================================================
// Bed.cs
// QUE HACE: la cama del dormitorio. Un tap acuesta a Poku o lo despierta.
// DONDE ESTA: en "Habitaciones/Dormitorio/Cama" de SampleScene (usa TouchTarget).
// PARA CAMBIAR: cuanta energia recupera durmiendo -> NeedsConfig.asset
//               ("Energia Durmiendo Por Hora"). El sonido -> sonidoDormir.
// ============================================================================

// Cama del dormitorio: un tap acuesta o despierta a la mascota. Mientras duerme, NeedsSystem
// recupera energia en vez de gastarla, y la despierta solo al llegar a 100.
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

    // Tap en la cama: invierte el estado (dormido <-> despierto).
    private void AlTocar()
    {
        if (necesidades == null) return;
        necesidades.Dormir(!necesidades.Dormido);
        // Solo al acostarlo: despertarlo no lleva sonido de cuna.
        if (necesidades.Dormido) SoundPlayer.Reproducir(sonidoDormir);
    }
}
