using UnityEngine;

// ============================================================================
// MusicaDeEscena.cs
// QUE HACE: al empezar la escena le dice al MusicPlayer que tema tiene que sonar.
// DONDE ESTA: en el objeto "Musica" de cada escena (la casa y cada minijuego).
// PARA CAMBIAR: la cancion de una escena -> campo "Tema" en el Inspector de ese
//               objeto "Musica".
// ============================================================================

// Se pone una vez en cada escena (o panel) y le dice al MusicPlayer que tema tiene que sonar.
public class MusicaDeEscena : MonoBehaviour
{
    [Header("Musica")]
    // [AJUSTE] El AudioClip que suena en esta escena (se arrastra en el Inspector).
    [SerializeField] private AudioClip tema;

    private void Start()
    {
        MusicPlayer.Cambiar(tema);
    }
}
