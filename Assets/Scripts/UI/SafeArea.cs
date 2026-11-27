using UnityEngine;

// ============================================================================
// SafeArea.cs
// QUE HACE: ajusta este objeto de UI a la "zona segura" del celular (sin el
//           notch, la camara ni los bordes redondeados), para que ningun boton
//           quede tapado.
// DONDE ESTA: en "Canvas/SafeArea" de cada escena y en algunos paneles.
// PARA CAMBIAR: nada. Todo lo que tiene que quedar visible va como hijo de SafeArea.
// ============================================================================

// Ajusta este RectTransform a la zona segura de la pantalla (sin notch ni bordes redondeados).
// Todo el HUD va adentro; el fondo del juego sigue usando la pantalla entera.
[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    // Lo ultimo aplicado, para no recalcular si no cambio nada.
    private Rect ultimaSafeArea;
    private int ultimoAncho;
    private int ultimoAlto;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Aplicar();
    }

    private void Update()
    {
        // La zona segura cambia en runtime, por ejemplo cuando aparece o se oculta la barra del
        // sistema. Solo se recalcula si algo cambio.
        if (ultimaSafeArea != Screen.safeArea || ultimoAncho != Screen.width || ultimoAlto != Screen.height)
        {
            Aplicar();
        }
    }

    // Pone los anchors del RectTransform en los bordes de la zona segura.
    private void Aplicar()
    {
        Rect safeArea = Screen.safeArea;

        // Screen.safeArea viene en pixeles; los anchors van de 0 a 1, asi que se normaliza.
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        ultimaSafeArea = safeArea;
        ultimoAncho = Screen.width;
        ultimoAlto = Screen.height;
    }
}
