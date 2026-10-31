using UnityEngine;

/// El cuadro colgado en la casa: muestra el ultimo dibujo guardado y un tap abre el atril.
[RequireComponent(typeof(TouchTarget))]
public class HouseFrame : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ScreenStack pantallas;
    [SerializeField] private GameObject pantallaDibujo;
    // Va adentro del marco, un poco por detras, con el tamano del hueco.
    [SerializeField] private SpriteRenderer dibujo;

    [Header("Hueco del marco")]
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

    private void Start()
    {
        Texture2D guardado = DrawingStorage.Cargar();
        if (guardado == null) return;
        Mostrar(guardado);
        Destroy(guardado);
    }

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
