using UnityEngine;

// Vibracion del celular. En el Editor y en otras plataformas no hace nada, asi que se puede llamar
// desde cualquier lado sin chequear la plataforma.
public static class Vibracion
{
    public static void Vibrar()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }
}
