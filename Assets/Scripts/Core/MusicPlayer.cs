using UnityEngine;

/// Musica de fondo. Una sola instancia que sobrevive a los cambios de escena y mezcla dos
/// AudioSource para pasar de un tema a otro con fundido cruzado, sin corte ni silencio en el medio.
public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instancia;

    [Header("Volumen")]
    [SerializeField] [Range(0f, 1f)] private float volumen = 0.4f;

    [Header("Fundido")]
    [SerializeField] private float segundosFundido = 0.8f;

    private AudioSource[] fuentes;
    private int actual;

    public static void Cambiar(AudioClip clip)
    {
        if (clip == null) return;

        if (instancia == null)
        {
            // Si nadie lo puso en la escena, se crea con los valores por defecto.
            new GameObject("MusicPlayer").AddComponent<MusicPlayer>();
        }

        instancia.Pedir(clip);
    }

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        fuentes = new AudioSource[2];
        for (int i = 0; i < fuentes.Length; i++)
        {
            fuentes[i] = gameObject.AddComponent<AudioSource>();
            fuentes[i].playOnAwake = false;
            fuentes[i].loop = true;
            fuentes[i].volume = 0f;
        }
    }

    private void Pedir(AudioClip clip)
    {
        // Mismo tema: no se reinicia, asi volver a una escena con la misma musica no corta el loop.
        if (fuentes[actual].clip == clip && fuentes[actual].isPlaying) return;

        actual = 1 - actual;
        AudioSource entrante = fuentes[actual];
        entrante.clip = clip;
        entrante.volume = 0f;
        entrante.Play();
    }

    // unscaledDeltaTime para que el fundido siga andando si el juego se pausa con timeScale en 0.
    private void Update()
    {
        float paso = volumen / Mathf.Max(0.01f, segundosFundido) * Time.unscaledDeltaTime;

        for (int i = 0; i < fuentes.Length; i++)
        {
            AudioSource fuente = fuentes[i];
            if (!fuente.isPlaying) continue;

            if (i == actual)
            {
                fuente.volume = Mathf.MoveTowards(fuente.volume, volumen, paso);
            }
            else
            {
                fuente.volume = Mathf.MoveTowards(fuente.volume, 0f, paso);
                if (fuente.volume <= 0f) fuente.Stop();
            }
        }
    }
}
