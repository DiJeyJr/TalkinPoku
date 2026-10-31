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

#if ADMIN
        // Build de prueba (se compila con el simbolo ADMIN): el boton se ve y abre con un toque, para
        // probar en el celular sin acordarse del truco. La build de entrega no lo tiene.
        toquesNecesarios = 1;
        GetComponent<Image>().color = new Color(0.93f, 0.54f, 0.18f, 0.85f);
        TMPro.TextMeshProUGUI etiqueta = new GameObject("Etiqueta", typeof(RectTransform)).AddComponent<TMPro.TextMeshProUGUI>();
        etiqueta.rectTransform.SetParent(transform, false);
        etiqueta.rectTransform.anchorMin = Vector2.zero;
        etiqueta.rectTransform.anchorMax = Vector2.one;
        etiqueta.rectTransform.sizeDelta = Vector2.zero;
        etiqueta.text = "Admin";
        etiqueta.fontSize = 34;
        etiqueta.fontStyle = TMPro.FontStyles.Bold;
        etiqueta.alignment = TMPro.TextAlignmentOptions.Center;
        etiqueta.raycastTarget = false;
#endif
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
