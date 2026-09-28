using UnityEngine;
using UnityEngine.UI;

/// Boton invisible arriba a la derecha: siete toques seguidos abren o cierran el panel de admin.
/// Seguidos quiere decir sin pausas largas, asi un toque suelto de vez en cuando no va sumando.
[RequireComponent(typeof(Button))]
public class AdminTrigger : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private AdminPanel panel;

    [Header("Umbrales")]
    [SerializeField] private int toquesNecesarios = 7;
    [SerializeField] private float pausaMaxima = 0.6f;

    private int toques;
    private float ultimoToque;

    private void Awake()
    {
        // La Image del boton es transparente pero con Raycast Target prendido: no se ve y recibe toques.
        GetComponent<Button>().onClick.AddListener(Tocar);
    }

    private void Tocar()
    {
        if (Time.time - ultimoToque > pausaMaxima) toques = 0;
        ultimoToque = Time.time;
        toques++;

        if (toques < toquesNecesarios) return;
        toques = 0;
        panel.Alternar();
    }
}
