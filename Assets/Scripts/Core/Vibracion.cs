using UnityEngine;

/// Vibracion del celular. En el Editor y en otras plataformas no hace nada, asi que se puede llamar
/// desde cualquier lado sin chequear la plataforma.
public static class Vibracion
{
    // Interruptor para una futura pantalla de opciones.
    public static bool Activada = true;

    public static void Vibrar()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Activada) Handheld.Vibrate();
#endif
    }
}
