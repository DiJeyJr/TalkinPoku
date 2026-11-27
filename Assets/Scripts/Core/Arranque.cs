using UnityEngine;

// ============================================================================
// Arranque.cs
// QUE HACE: configura cosas generales del juego antes de que cargue la primera
//           escena (por ahora, los cuadros por segundo).
// DONDE ESTA: en ningun objeto. Unity lo ejecuta solo al abrir el juego gracias
//             al atributo [RuntimeInitializeOnLoadMethod].
// PARA CAMBIAR: los FPS del juego -> la constante FpsObjetivo (marcada [AJUSTE]).
// ============================================================================

// Ajustes que tienen que estar antes de la primera escena, sin depender de que haya un objeto puesto.
public static class Arranque
{
    // [AJUSTE] Cuadros por segundo del juego.
    // En Android el default es 30 fps y se nota en los arrastres y el swipe. vSync en 0 porque si
    // un perfil de calidad lo tiene prendido, targetFrameRate se ignora.
    private const int FpsObjetivo = 60;

    // Se ejecuta una sola vez, antes de cargar la primera escena.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Configurar()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = FpsObjetivo;
    }
}
