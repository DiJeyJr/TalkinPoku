using UnityEngine;

/// Se pone una vez en cada escena (o panel) y le dice al MusicPlayer que tema tiene que sonar.
public class MusicaDeEscena : MonoBehaviour
{
    [Header("Musica")]
    [SerializeField] private AudioClip tema;

    private void Start()
    {
        MusicPlayer.Cambiar(tema);
    }
}
