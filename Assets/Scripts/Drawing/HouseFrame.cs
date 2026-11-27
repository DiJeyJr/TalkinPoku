using UnityEngine;

// ============================================================================
// HouseFrame.cs
// QUE HACE: el cuadro de la Sala de arte. Muestra el ultimo dibujo guardado y,
//           con un tap, abre la pantalla de dibujo.
// DONDE ESTA: en "Habitaciones/Sala de arte/CuadroDibujo" de SampleScene.
// PARA CAMBIAR: tamano del dibujo dentro del marco -> anchoHueco.
//               posicion/tamano del cuadro -> el Transform de CuadroDibujo.
// ============================================================================

// El cuadro colgado en la casa: muestra el ultimo dibujo guardado y un tap abre el atril.
[RequireComponent(typeof(TouchTarget))]
public class HouseFrame : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ScreenStack pantallas;
    [SerializeField] private GameObject pantallaDibujo;
    // Va adentro del marco, un poco por detras, con el tamano del hueco.
    [SerializeField] private SpriteRenderer dibujo;

    [Header("Hueco del marco")]
    // [AJUSTE] Ancho del dibujo dentro del marco.
    // Ancho del hueco en unidades locales del marco: el dibujo se escala para llenarlo.
    [SerializeField] private float anchoHueco = 15f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoTocar;

    private TouchTarget toque;
    private Texture2D textura;

    private void Awake()
    {
        toque = GetComponent<TouchTarget>();
    }

    private void OnEnable()
    {
        toque.Tocado += AlTocar;
        DrawingStorage.Guardado += Mostrar;
    }

    private void OnDisable()
    {
        toque.Tocado -= AlTocar;
        DrawingStorage.Guardado -= Mostrar;
    }

    // Al arrancar muestra el dibujo guardado (si hay).
    private void Start()
    {
        Texture2D guardado = DrawingStorage.Cargar();
        if (guardado == null) return;
        Mostrar(guardado);
        Destroy(guardado);
    }

    // Tap en el cuadro: abre la pantalla de dibujo.
    private void AlTocar()
    {
        SoundPlayer.Reproducir(sonidoTocar);
        pantallas.Abrir(pantallaDibujo);
    }

    // Se copia la textura: la de la hoja se sigue pisando mientras dibuja.
    private void Mostrar(Texture2D nuevo)
    {
        if (textura == null) textura = new Texture2D(nuevo.width, nuevo.height, TextureFormat.RGBA32, false);
        textura.SetPixels32(nuevo.GetPixels32());
        textura.Apply(false);

        if (dibujo.sprite != null) Destroy(dibujo.sprite);
        dibujo.sprite = Sprite.Create(textura, new Rect(0, 0, textura.width, textura.height), new Vector2(0.5f, 0.5f), textura.width / anchoHueco);
    }

    private void OnDestroy()
    {
        if (textura != null) Destroy(textura);
    }
}
