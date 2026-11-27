using TMPro;
using UnityEngine;

// ============================================================================
// LevelUpBanner.cs
// QUE HACE: el cartel que aparece unos segundos al subir de nivel ("¡Nivel 5!").
//           PetEvolution tambien lo usa para avisar evoluciones y la Mega.
// DONDE ESTA: en "Canvas/CartelNivel" de SampleScene.
// PARA CAMBIAR: cuanto tiempo se ve -> segundosVisible.
//               el texto al subir de nivel -> AlSubir().
// USO DESDE OTRO SCRIPT: cartel.Mostrar("cualquier mensaje");
// ============================================================================

// Cartel que aparece unos segundos al subir de nivel. PetEvolution lo usa tambien para avisar que
// la mascota evoluciono: como eso pasa justo despues de subir, su mensaje tapa al del nivel.
public class LevelUpBanner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Progression progreso;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text texto;
    // Opcional: el cartel entra con un latido.
    [SerializeField] private Latido latido;

    [Header("Feedback")]
    // [AJUSTE] Segundos que queda visible.
    [SerializeField] private float segundosVisible = 2.5f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoNivel;

    private float ocultarEn;

    private void Awake()
    {
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        progreso.NivelSubido += AlSubir;
    }

    private void OnDisable()
    {
        progreso.NivelSubido -= AlSubir;
    }

    // Lo esconde cuando pasa el tiempo.
    private void Update()
    {
        if (panel.activeSelf && Time.time >= ocultarEn) panel.SetActive(false);
    }

    // Muestra un mensaje en el cartel.
    public void Mostrar(string mensaje)
    {
        texto.SetText(mensaje);
        panel.SetActive(true);
        ocultarEn = Time.time + segundosVisible;
        if (latido != null) latido.Latir();
    }

    private void AlSubir(int nivel)
    {
        SoundPlayer.Reproducir(sonidoNivel);
        Mostrar("¡Nivel " + nivel + "!");
    }
}
