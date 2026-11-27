using UnityEngine;

// ============================================================================
// SoundPlayer.cs
// QUE HACE: reproduce los efectos de sonido (pops, festejos, etc).
// DONDE ESTA: en ningun objeto. Es una clase estatica: cualquier script llama
//             SoundPlayer.Reproducir(clip) y se crea solo la primera vez.
// PARA CAMBIAR: que sonido hace algo -> el campo de AudioClip del script que lo
//               dispara (por ejemplo PetJuice o BotonFeedback), en el Inspector.
//               El volumen de un sonido puntual -> el segundo parametro de
//               Reproducir(clip, volumen).
// ============================================================================

// Reproduce los efectos de sonido. Una sola AudioSource para todo el juego: PlayOneShot deja que
// varios suenen a la vez sin cortarse. Se crea al primer uso y sobrevive a los cambios de escena,
// asi el pop del boton que carga un minijuego no se corta a la mitad.
public static class SoundPlayer
{
    private static AudioSource fuente;

    // volumen va de 0 a 1; si no se pasa, suena al maximo.
    public static void Reproducir(AudioClip clip, float volumen = 1f)
    {
        // Sin clip asignado no suena y no rompe nada: el sonido nunca es parte de la logica.
        if (clip == null) return;

        // La primera vez crea el objeto con el parlante.
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
