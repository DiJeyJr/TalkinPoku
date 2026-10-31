using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DrawingTool { Lapiz, Pincel, Borrador }

/// La hoja del atril: pinta sobre una textura siguiendo el dedo. Entre dos eventos de arrastre el
/// dedo puede saltar varios pixeles, asi que se estampan circulos a lo largo de la linea: si no,
/// un trazo rapido queda como puntos sueltos.
[RequireComponent(typeof(RawImage))]
public class DrawingCanvas : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("Pincel")]
    [SerializeField] private Color fondo = Color.white;
    // El lapiz es fino siempre; el grosor elegido es para el pincel y el borrador.
    [SerializeField] private int radioLapiz = 2;

    public DrawingTool Herramienta { get; set; } = DrawingTool.Pincel;
    public Color ColorActual { get; set; } = Color.black;
    public int Grosor { get; set; } = 8;
    public Texture2D Textura => textura;

    private RawImage imagen;
    private Texture2D textura;
    private Color32[] pixeles;
    private Vector2 anterior;

    private void Awake()
    {
        imagen = GetComponent<RawImage>();
        textura = new Texture2D(DrawingStorage.Ancho, DrawingStorage.Alto, TextureFormat.RGBA32, false);
        textura.wrapMode = TextureWrapMode.Clamp;
        pixeles = new Color32[textura.width * textura.height];
        imagen.texture = textura;
        Limpiar();
    }

    private void OnDestroy()
    {
        if (textura != null) Destroy(textura);
    }

    public void Limpiar()
    {
        Color32 c = fondo;
        for (int i = 0; i < pixeles.Length; i++) pixeles[i] = c;
        Aplicar();
    }

    // Arranca desde el dibujo guardado, para seguirlo en vez de empezar de cero.
    public void Cargar(Texture2D guardado)
    {
        if (guardado == null || guardado.width != textura.width || guardado.height != textura.height)
        {
            Limpiar();
            return;
        }
        pixeles = guardado.GetPixels32();
        Aplicar();
    }

    public void OnPointerDown(PointerEventData evento)
    {
        if (!APixel(evento, out anterior)) return;
        Estampar(anterior);
        Aplicar();
    }

    public void OnDrag(PointerEventData evento)
    {
        if (!APixel(evento, out Vector2 actual)) return;
        float distancia = Vector2.Distance(anterior, actual);
        int pasos = Mathf.Max(1, Mathf.CeilToInt(distancia / Mathf.Max(1f, Radio() * 0.5f)));
        for (int i = 1; i <= pasos; i++) Estampar(Vector2.Lerp(anterior, actual, (float)i / pasos));
        anterior = actual;
        Aplicar();
    }

    // Publico para probarlo sin dedo: un trazo de un punto a otro, en fracciones de la hoja.
    public void Trazar(Vector2 desde, Vector2 hasta)
    {
        Vector2 a = Vector2.Scale(desde, new Vector2(textura.width, textura.height));
        Vector2 b = Vector2.Scale(hasta, new Vector2(textura.width, textura.height));
        int pasos = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(a, b) / Mathf.Max(1f, Radio() * 0.5f)));
        for (int i = 0; i <= pasos; i++) Estampar(Vector2.Lerp(a, b, (float)i / pasos));
        Aplicar();
    }

    private int Radio()
    {
        if (Herramienta == DrawingTool.Lapiz) return radioLapiz;
        if (Herramienta == DrawingTool.Borrador) return Mathf.RoundToInt(Grosor * 1.5f);
        return Grosor;
    }

    private bool APixel(PointerEventData evento, out Vector2 pixel)
    {
        pixel = Vector2.zero;
        RectTransform rect = imagen.rectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, evento.position, evento.pressEventCamera, out Vector2 local)) return false;
        Vector2 fraccion = Rect.PointToNormalized(rect.rect, local);
        pixel = new Vector2(fraccion.x * textura.width, fraccion.y * textura.height);
        return true;
    }

    private void Estampar(Vector2 centro)
    {
        Color32 c = Herramienta == DrawingTool.Borrador ? (Color32)fondo : (Color32)ColorActual;
        int r = Radio();
        int cx = Mathf.RoundToInt(centro.x);
        int cy = Mathf.RoundToInt(centro.y);
        int ancho = textura.width;
        for (int y = Mathf.Max(0, cy - r); y <= Mathf.Min(textura.height - 1, cy + r); y++)
        {
            for (int x = Mathf.Max(0, cx - r); x <= Mathf.Min(ancho - 1, cx + r); x++)
            {
                int dx = x - cx;
                int dy = y - cy;
                if (dx * dx + dy * dy <= r * r) pixeles[y * ancho + x] = c;
            }
        }
    }

    private void Aplicar()
    {
        textura.SetPixels32(pixeles);
        textura.Apply(false);
    }
}
