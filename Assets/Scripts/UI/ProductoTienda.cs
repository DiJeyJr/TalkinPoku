using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Un ingrediente a la venta: dibujo, nombre, precio y cuantos tenes. Cada toque compra uno.
public class ProductoTienda : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image dibujo;
    [SerializeField] private TMP_Text nombre;
    [SerializeField] private TMP_Text precio;
    [SerializeField] private TMP_Text cantidad;
    [SerializeField] private Button boton;
    [SerializeField] private Latido latido;

    [Header("Colores")]
    [SerializeField] private Color colorPrecio = new Color(0.35f, 0.2f, 0.13f);
    [SerializeField] private Color colorSinPlata = new Color(0.85f, 0.2f, 0.15f);

    [Header("Feedback")]
    [SerializeField] private float segundosSinPlata = 0.6f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoCompra;
    [SerializeField] private AudioClip sonidoSinPlata;

    private IngredientDefinition ingrediente;
    private Wallet billetera;
    private float volverColorEn = -1f;

    // La llama TiendaComida al armar la pestana: el mismo prefab sirve para cualquier ingrediente.
    public void Configurar(IngredientDefinition definicion, Wallet billeteraTienda)
    {
        ingrediente = definicion;
        billetera = billeteraTienda;

        dibujo.sprite = definicion.Sprite;
        nombre.SetText(definicion.Nombre);
        precio.SetText("{0}", definicion.Precio);
        precio.color = colorPrecio;
        cantidad.SetText("x{0}", Despensa.Ingredientes.Cantidad(definicion.Id));
        boton.onClick.AddListener(Comprar);
    }

    private void OnEnable()
    {
        Despensa.Ingredientes.Cambio += AlCambiar;
        // Mientras la tienda estaba cerrada se pudo cocinar y gastar: al abrirla se relee.
        if (ingrediente != null) cantidad.SetText("x{0}", Despensa.Ingredientes.Cantidad(ingrediente.Id));
    }

    private void OnDisable()
    {
        Despensa.Ingredientes.Cambio -= AlCambiar;
    }

    private void Update()
    {
        if (volverColorEn < 0f || Time.unscaledTime < volverColorEn) return;
        volverColorEn = -1f;
        precio.color = colorPrecio;
    }

    private void AlCambiar(string id, int cantidadNueva)
    {
        if (ingrediente != null && id == ingrediente.Id) cantidad.SetText("x{0}", cantidadNueva);
    }

    private void Comprar()
    {
        if (!billetera.Gastar(CurrencyType.Monedas, ingrediente.Precio))
        {
            // No alcanza: el precio se pone rojo un momento y suena distinto. Sin carteles: el chico
            // ve el numero en rojo y entiende.
            precio.color = colorSinPlata;
            volverColorEn = Time.unscaledTime + segundosSinPlata;
            SoundPlayer.Reproducir(sonidoSinPlata);
            return;
        }

        Despensa.Ingredientes.Sumar(ingrediente.Id);
        SoundPlayer.Reproducir(sonidoCompra);
        if (latido != null) latido.Latir();
        // Es una compra: se guarda ya y no en el proximo autoguardado.
        SaveSystem.Guardar();
    }
}
