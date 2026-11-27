using System;
using UnityEngine;

// ============================================================================
// ItemDefinition.cs
// QUE HACE: describe un item: nombre, icono, precio, con que se paga, desde que
//           nivel se desbloquea, cuanto llena si es comida, y como se ve si es
//           ropa o un mueble.
// DONDE ESTA: cada item es un asset en Data/Items (Item_Manzana, Item_Ropa_*,
//             Item_Mueble_*...). Los valores se cambian en el Inspector del asset.
// PARA CAMBIAR:
//   - precio -> "Precio" | moneda -> "Moneda" | nivel de desbloqueo -> "Nivel Requerido"
//   - cuanto llena una comida -> "Cantidad" (y que necesidad llena -> "Necesidad")
//   - tamano/posicion de ropa -> "Escala Mundo", "Desplazamiento" y "Ajustes" por etapa
//   - lugar y posicion de un mueble -> "Lugar" y "Posicion Inicial"
// OJO: los numeros de abajo son solo los valores por defecto al crear un item
//      nuevo. El valor que usa el juego es el del asset.
// ============================================================================

// Un objeto del juego: comida, ropa o un mueble. El id es lo que va a ir al guardado: si se renombra
// el asset, las partidas guardadas siguen funcionando.
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
    // Texto unico que se guarda en la partida. No cambiarlo en items ya usados.
    [SerializeField] private string id;
    // [AJUSTE] Nombre que se muestra en la tienda.
    [SerializeField] private string nombre;
    [SerializeField] private Sprite icono;
    [SerializeField] private ItemCategory categoria = ItemCategory.Comida;

    [Header("Tienda")]
    // [AJUSTE] Precio en la tienda.
    // 0 = no se vende (la manzana de la cocina es gratis).
    [SerializeField] private int precio;
    // [AJUSTE] Con que se paga (Monedas o Gemas).
    [SerializeField] private CurrencyType moneda = CurrencyType.Monedas;
    // [AJUSTE] Nivel minimo para comprarlo.
    // Hasta este nivel aparece en la tienda con candado. La ropa se desbloquea por etapa: asi
    // evolucionar trae cosas nuevas para ponerle.
    [SerializeField] private int nivelRequerido = 1;

    [Header("Comida")]
    // [AJUSTE] Que necesidad llena al comerla y cuanto (de 0 a 100).
    [SerializeField] private NeedType necesidad = NeedType.Hambre;
    [SerializeField] private float cantidad = 20f;

    [Header("Ropa y muebles")]
    // Como se ve puesto en Poku o colocado en la sala. El icono es solo para la UI.
    [SerializeField] private Sprite spriteMundo;
    // [AJUSTE] Tamano del sprite en la escena.
    [SerializeField] private float escalaMundo = 1f;
    // Donde se pone en Poku (solo ropa).
    [SerializeField] private AccessorySlot ranura = AccessorySlot.Cabeza;
    // Corrimiento del accesorio respecto del ancla, en todas las etapas: una vincha va mas abajo que
    // una gorra aunque las dos sean de la cabeza. Se achica con la escala de accesorios de la etapa.
    [SerializeField] private Vector2 desplazamiento;
    // Correcciones de posicion/tamano para una evolucion puntual (ver AjusteEtapa arriba).
    [SerializeField] private AjusteEtapa[] ajustes;
    [Header("Muebles")]
    // Los muebles no se arrastran: cada uno tiene su lugar en la sala.
    [SerializeField] private FurnitureSpot lugar;
    // [AJUSTE] Posicion en la sala.
    // Donde va en la sala, en unidades de mundo. Los cuadros usan los lugares de RoomDecor.
    [SerializeField] private Vector2 posicionInicial;
    // El sillon y la mesita miran hacia Poku, que esta en el medio.
    [SerializeField] private bool espejado;
    // Orden de dibujo entre muebles: la alfombra abajo de todo, la planta adelante.
    [SerializeField] private int capa;

    // Propiedades de solo lectura: los demas scripts leen los valores por aca.
    public string Id => id;
    public string Nombre => nombre;
    public Sprite Icono => icono;
    public ItemCategory Categoria => categoria;
    public int Precio => precio;
    public CurrencyType Moneda => moneda;
    public int NivelRequerido => nivelRequerido;
    // true si el jugador todavia no llego al nivel necesario.
    public bool Bloqueado => Progression.NivelActual < nivelRequerido;
    public NeedType Necesidad => necesidad;
    public float Cantidad => cantidad;
    public Sprite SpriteMundo => spriteMundo;
    public float EscalaMundo => escalaMundo;
    public AccessorySlot Ranura => ranura;
    public Vector2 Desplazamiento => desplazamiento;

    // Devuelve la correccion para una evolucion; si no hay, una neutra (sin corrimiento, escala 1).
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
