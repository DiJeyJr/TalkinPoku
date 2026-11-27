using System;
using System.IO;
using UnityEngine;

// ============================================================================
// DrawingStorage.cs
// QUE HACE: guarda y carga el dibujo del cuadro como una imagen PNG
//           (cuadro.png, al lado de partida.json).
// DONDE ESTA: en ningun objeto (clase estatica).
// PARA CAMBIAR: tamano de la hoja en pixeles -> Ancho y Alto.
// AVISA: Guardado(textura) cuando se guarda un dibujo nuevo (lo escucha HouseFrame).
// ============================================================================

// Guarda el dibujo del cuadro como PNG aparte del save en JSON: una imagen en base64 adentro del
// JSON lo haria pesado de escribir en cada autoguardado, y el dibujo solo cambia al tocar Guardar.
public static class DrawingStorage
{
    // [AJUSTE] Resolucion de la hoja de dibujo (si se cambia, los dibujos viejos no cargan).
    public const int Ancho = 512;
    public const int Alto = 384;

    // El cuadro de la casa lo escucha para mostrar el dibujo nuevo sin recargar la escena.
    public static event Action<Texture2D> Guardado;

    private static string Ruta => Path.Combine(Application.persistentDataPath, "cuadro.png");

    // Escribe el dibujo como PNG y avisa.
    public static void Guardar(Texture2D dibujo)
    {
        try
        {
            File.WriteAllBytes(Ruta, dibujo.EncodeToPNG());
        }
        catch (Exception e)
        {
            Debug.LogWarning("No se pudo guardar el dibujo: " + e.Message);
            return;
        }
        Guardado?.Invoke(dibujo);
    }

    // Lee el PNG guardado y lo devuelve como textura.
    // Null si todavia no dibujo nada.
    public static Texture2D Cargar()
    {
        if (!File.Exists(Ruta)) return null;
        Texture2D textura = new Texture2D(Ancho, Alto, TextureFormat.RGBA32, false);
        try
        {
            if (textura.LoadImage(File.ReadAllBytes(Ruta))) return textura;
        }
        catch (Exception e)
        {
            Debug.LogWarning("No se pudo leer el dibujo: " + e.Message);
        }
        UnityEngine.Object.Destroy(textura);
        return null;
    }
}
