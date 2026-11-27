using UnityEngine;

// ============================================================================
// Vibracion.cs
// QUE HACE: hace vibrar el celular. Se llama con Vibracion.Vibrar() desde
//           cualquier script.
// DONDE ESTA: en ningun objeto (clase estatica).
// PARA CAMBIAR: donde vibra -> buscar las llamadas a Vibracion.Vibrar() en los
//               demas scripts.
// ============================================================================

// Vibracion del celular. En el Editor y en otras plataformas no hace nada, asi que se puede llamar
// desde cualquier lado sin chequear la plataforma.
public static class Vibracion
{
    public static void Vibrar()
    {
        // Solo se compila en Android e iOS; en el Editor esta linea no existe.
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }
}
