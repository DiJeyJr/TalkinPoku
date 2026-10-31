using UnityEngine;
using UnityEngine.UI;

/// La pantalla del atril: herramientas, colores y grosores alrededor de la hoja. Al abrirse carga
/// lo que ya estaba dibujado; Guardar lo cuelga en el cuadro de la casa y cierra.
public class DrawingScreen : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ScreenStack pantallas;
    [SerializeField] private DrawingCanvas hoja;
    [SerializeField] private Button pincel;
    [SerializeField] private Button borrador;
    [SerializeField] private Button limpiar;
    [SerializeField] private Button guardar;
    [SerializeField] private Button cerrar;
    // Uno por grosor, de finito a grueso: transparentes, encima de los tres circulos del dibujo.
    [SerializeField] private Button[] grosores;
    // Una muestra por color: el boton blanco de Martina tenido de cada color.
    [SerializeField] private Button plantillaColor;
    [SerializeField] private Transform paleta;

    [Header("Opciones")]
    [SerializeField] private int[] radios = { 4, 8, 14 };
    [SerializeField] private Color[] colores =
    {
        new Color(0.13f, 0.1f, 0.15f), new Color(0.93f, 0.26f, 0.26f), new Color(1f, 0.6f, 0.15f),
        new Color(1f, 0.85f, 0.2f), new Color(0.35f, 0.75f, 0.3f), new Color(0.25f, 0.55f, 0.95f),
        new Color(0.62f, 0.38f, 0.85f), new Color(1f, 0.55f, 0.75f), new Color(0.55f, 0.35f, 0.2f),
    };
    [SerializeField] private float escalaElegido = 1.2f;
    // Los botones de grosor son invisibles encima del dibujo: agrandarlos no se ve. Se oscurecen los
    // que no estan elegidos.
    [SerializeField] private Color grosorApagado = new Color(0f, 0f, 0f, 0.45f);

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoGuardar;
    [SerializeField] private AudioClip sonidoTocar;

    private Button[] muestras;

    private void Awake()
    {
        pincel.onClick.AddListener(() => ElegirHerramienta(DrawingTool.Pincel));
        borrador.onClick.AddListener(() => ElegirHerramienta(DrawingTool.Borrador));
        limpiar.onClick.AddListener(() =>
        {
            hoja.Limpiar();
            SoundPlayer.Reproducir(sonidoTocar);
        });
        guardar.onClick.AddListener(Guardar);
        cerrar.onClick.AddListener(() => pantallas.Cerrar());

        for (int i = 0; i < grosores.Length; i++)
        {
            int cual = i;
            grosores[i].onClick.AddListener(() => ElegirGrosor(cual));
        }

        plantillaColor.gameObject.SetActive(false);
        muestras = new Button[colores.Length];
        for (int i = 0; i < colores.Length; i++)
        {
            int cual = i;
            Button muestra = Instantiate(plantillaColor, paleta);
            muestra.gameObject.SetActive(true);
            muestra.image.color = colores[i];
            muestra.onClick.AddListener(() => ElegirColor(cual));
            muestras[i] = muestra;
        }
    }

    private void OnEnable()
    {
        Texture2D guardado = DrawingStorage.Cargar();
        hoja.Cargar(guardado);
        if (guardado != null) Destroy(guardado);
        ElegirHerramienta(DrawingTool.Pincel);
        ElegirGrosor(1);
        ElegirColor(0);
    }

    private void ElegirHerramienta(DrawingTool herramienta)
    {
        hoja.Herramienta = herramienta;
        Resaltar(pincel, herramienta == DrawingTool.Pincel);
        Resaltar(borrador, herramienta == DrawingTool.Borrador);
    }

    private void ElegirGrosor(int cual)
    {
        hoja.Grosor = radios[Mathf.Clamp(cual, 0, radios.Length - 1)];
        for (int i = 0; i < grosores.Length; i++) grosores[i].image.color = i == cual ? Color.clear : grosorApagado;
    }

    private void ElegirColor(int cual)
    {
        hoja.ColorActual = colores[cual];
        for (int i = 0; i < muestras.Length; i++) Resaltar(muestras[i], i == cual);
        // Elegir un color con el borrador puesto confunde: se vuelve al pincel.
        if (hoja.Herramienta == DrawingTool.Borrador) ElegirHerramienta(DrawingTool.Pincel);
    }

    private void Resaltar(Button boton, bool elegido)
    {
        boton.transform.localScale = Vector3.one * (elegido ? escalaElegido : 1f);
    }

    private void Guardar()
    {
        DrawingStorage.Guardar(hoja.Textura);
        SoundPlayer.Reproducir(sonidoGuardar);
        pantallas.Cerrar();
    }
}
