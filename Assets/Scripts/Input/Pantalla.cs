using UnityEngine;

// ============================================================================
// Pantalla.cs
// QUE HACE: convierte milimetros a pixeles segun la densidad (dpi) del celular.
//           Lo usan los detectores de gestos para que el dedo tenga que moverse
//           lo mismo en cualquier pantalla.
// DONDE ESTA: en ningun objeto (clase estatica).
// PARA CAMBIAR: normalmente nada; los umbrales de cada gesto estan en
//               TouchTarget, Draggable y SwipeDetector.
// ============================================================================

// Los umbrales de clase estan en pixeles, pero 80 px son 5 mm en un celular de 400 dpi y 13 mm en
// uno de 160 dpi. Medidos en milimetros, el gesto pide el mismo recorrido del dedo en cualquier pantalla.
public static class Pantalla
{
    // Algunos dispositivos devuelven 0 en Screen.dpi. 160 es la densidad base de Android.
    private const float DpiPorDefecto = 160f;
    private const float MmPorPulgada = 25.4f;

    // pixeles = mm * puntos por pulgada / mm por pulgada.
    public static float MmAPixeles(float mm)
    {
        float dpi = Screen.dpi > 0f ? Screen.dpi : DpiPorDefecto;
        return mm * dpi / MmPorPulgada;
    }
}
