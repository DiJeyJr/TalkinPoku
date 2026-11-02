using UnityEngine;

/// Como MusicaDeEscena pero para paneles que se abren encima de la casa (la tienda). ScreenStack los
/// prende y apaga con SetActive, asi que alcanza con OnEnable y OnDisable para cambiar el tema.
public class MusicaDePanel : MonoBehaviour
{
    [Header("Musica")]
    [SerializeField] private AudioClip tema;
    // El que vuelve a sonar al cerrar el panel: el de la escena de abajo.
    [SerializeField] private AudioClip temaAlCerrar;

    private void OnEnable()
    {
        MusicPlayer.Cambiar(tema);
    }

    private void OnDisable()
    {
        MusicPlayer.Cambiar(temaAlCerrar);
    }
}
