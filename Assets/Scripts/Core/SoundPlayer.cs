using UnityEngine;

/// Reproduce los efectos de sonido. Una sola AudioSource para todo el juego: PlayOneShot deja que
/// varios suenen a la vez sin cortarse. Se crea al primer uso y sobrevive a los cambios de escena,
/// asi el pop del boton que carga un minijuego no se corta a la mitad.
public static class SoundPlayer
{
    private static AudioSource fuente;

    public static void Reproducir(AudioClip clip, float volumen = 1f)
    {
        // Sin clip asignado no suena y no rompe nada: el sonido nunca es parte de la logica.
        if (clip == null) return;

        if (fuente == null)
        {
            GameObject objeto = new GameObject("SoundPlayer");
            Object.DontDestroyOnLoad(objeto);
            fuente = objeto.AddComponent<AudioSource>();
            fuente.playOnAwake = false;
        }

        fuente.PlayOneShot(clip, volumen);
    }
}
