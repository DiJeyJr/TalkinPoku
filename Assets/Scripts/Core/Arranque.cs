using UnityEngine;

/// Ajustes que tienen que estar antes de la primera escena, sin depender de que haya un objeto puesto.
public static class Arranque
{
    // En Android el default es 30 fps y se nota en los arrastres y el swipe. vSync en 0 porque si
    // un perfil de calidad lo tiene prendido, targetFrameRate se ignora.
    private const int FpsObjetivo = 60;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Configurar()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = FpsObjetivo;
    }
}
