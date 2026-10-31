using System;
using UnityEngine;

/// Un objeto del juego: comida, ropa o un mueble. El id es lo que va a ir al guardado: si se renombra
/// el asset, las partidas guardadas siguen funcionando.
[CreateAssetMenu(menuName = "TalkinPoku/Item", fileName = "Item_")]
public class ItemDefinition : ScriptableObject
{
    // Cada evolucion tiene la cara con otra forma: los mismos anteojos que le quedan bien a Poku le
    // quedan grandes a Pokuchan. Esto corrige un accesorio en una etapa sin tocar las demas.
    [Serializable]
    public struct AjusteEtapa
    {
        public PetAppearance apariencia;
        // En unidades del sprite de la mascota, sumado al ancla de la ranura.
        public Vector2 desplazamiento;
        // Multiplica al tamano de siempre. 0 cuenta como 1, que es lo que pone el Inspector al agregar uno.
        public float escala;
    }

    [Header("Identidad")]
    [SerializeField] private string id;
    [SerializeField] private string nombre;
    [SerializeField] private Sprite icono;
    [SerializeField] private ItemCategory categoria = ItemCategory.Comida;

    [Header("Tienda")]
    // 0 = no se vende (la manzana de la cocina es gratis).
    [SerializeField] private int precio;
    [SerializeField] private CurrencyType moneda = CurrencyType.Monedas;
    // Hasta este nivel aparece en la tienda con candado. La ropa se desbloquea por etapa: asi
    // evolucionar trae cosas nuevas para ponerle.
    [SerializeField] private int nivelRequerido = 1;

    [Header("Comida")]
    [SerializeField] private NeedType necesidad = NeedType.Hambre;
    [SerializeField] private float cantidad = 20f;

    [Header("Ropa y muebles")]
    // Como se ve puesto en Poku o colocado en la sala. El icono es solo para la UI.
    [SerializeField] private Sprite spriteMundo;
    [SerializeField] private float escalaMundo = 1f;
    [SerializeField] private AccessorySlot ranura = AccessorySlot.Cabeza;
    // Corrimiento del accesorio respecto del ancla, en todas las etapas: una vincha va mas abajo que
    // una gorra aunque las dos sean de la cabeza. Se achica con la escala de accesorios de la etapa.
    [SerializeField] private Vector2 desplazamiento;
    [SerializeField] private AjusteEtapa[] ajustes;
    [Header("Muebles")]
    // Los muebles no se arrastran: cada uno tiene su lugar en la sala.
    [SerializeField] private FurnitureSpot lugar;
    // Donde va en la sala, en unidades de mundo. Los cuadros usan los lugares de RoomDecor.
    [SerializeField] private Vector2 posicionInicial;
    // El sillon y la mesita miran hacia Poku, que esta en el medio.
    [SerializeField] private bool espejado;
    // Orden de dibujo entre muebles: la alfombra abajo de todo, la planta adelante.
    [SerializeField] private int capa;

    public string Id => id;
    public string Nombre => nombre;
    public Sprite Icono => icono;
    public ItemCategory Categoria => categoria;
    public int Precio => precio;
    public CurrencyType Moneda => moneda;
    public int NivelRequerido => nivelRequerido;
    public bool Bloqueado => Progression.NivelActual < nivelRequerido;
    public NeedType Necesidad => necesidad;
    public float Cantidad => cantidad;
    public Sprite SpriteMundo => spriteMundo;
    public float EscalaMundo => escalaMundo;
    public AccessorySlot Ranura => ranura;
    public Vector2 Desplazamiento => desplazamiento;

    public AjusteEtapa AjusteDe(PetAppearance apariencia)
    {
        if (ajustes != null)
        {
            foreach (AjusteEtapa ajuste in ajustes)
            {
                if (ajuste.apariencia == apariencia) return ajuste;
            }
        }
        return new AjusteEtapa { apariencia = apariencia, escala = 1f };
    }

    public FurnitureSpot Lugar => lugar;
    public Vector2 PosicionInicial => posicionInicial;
    public bool Espejado => espejado;
    public int Capa => capa;

    // La comida se gasta al comerla: se puede tener varias. Ropa y muebles se compran una vez.
    public bool SeAcumula => categoria == ItemCategory.Comida;
}
