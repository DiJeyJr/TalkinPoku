using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// La bandeja de inventario de una habitacion: la comida en la cocina, la ropa en el dormitorio y los
/// muebles en la sala. El boton que la abre solo aparece en su habitacion. Se abre como una pantalla
/// del ScreenStack: asi se apaga el input de la casa y arrastrar comida no cambia de habitacion.
public class InventoryTray : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Inventory inventario;
    [SerializeField] private RoomNavigator navegador;
    [SerializeField] private ScreenStack pantallas;
    // El boton del HUD que la abre: se esconde en las otras habitaciones.
    [SerializeField] private GameObject boton;
    [SerializeField] private GameObject panel;
    [SerializeField] private InventorySlot plantilla;
    [SerializeField] private Transform contenedor;
    [SerializeField] private TMP_Text vacio;

    [Header("Bandeja")]
    [SerializeField] private ItemCategory categoria;
    [SerializeField] private int habitacion;

    [Header("Comida")]
    // Solo para la de la cocina: la copia del icono que sigue al dedo, y a quien se le da.
    [SerializeField] private Image fantasma;
    [SerializeField] private PetDropTarget mascota;
    [SerializeField] private PetMoodController cara;
    [SerializeField] private Progression progreso;
    [SerializeField] private float segundosContento = 1.5f;
    [SerializeField] private AudioClip sonidoComer;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoPoner;

    // La sala la escucha para dejar mover los muebles solo con la bandeja abierta.
    public event Action<bool> AbiertaCambiada;

    private readonly List<InventorySlot> slots = new List<InventorySlot>();
    private bool abiertaAntes;

    private void Awake()
    {
        plantilla.gameObject.SetActive(false);
        panel.SetActive(false);
        if (fantasma != null) fantasma.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        inventario.Cambiado += Refrescar;
        navegador.HabitacionCambiada += AlCambiarHabitacion;
    }

    private void OnDisable()
    {
        inventario.Cambiado -= Refrescar;
        navegador.HabitacionCambiada -= AlCambiarHabitacion;
    }

    private void Start()
    {
        AlCambiarHabitacion(navegador.Actual);
    }

    private void Update()
    {
        // El ScreenStack la puede cerrar con el boton atras de Android sin pasar por aca: se mira
        // si cambio en vez de esperar un aviso.
        if (abiertaAntes == panel.activeSelf) return;
        abiertaAntes = panel.activeSelf;
        if (abiertaAntes) Refrescar();
        else if (fantasma != null) fantasma.gameObject.SetActive(false);
        AbiertaCambiada?.Invoke(abiertaAntes);
    }

    // La llama el boton del HUD.
    public void Alternar()
    {
        if (panel.activeSelf) pantallas.Cerrar();
        else
        {
            pantallas.Abrir(panel);
            Refrescar();
        }
    }

    private void AlCambiarHabitacion(int actual)
    {
        // Los puntitos del indicador siguen andando con la bandeja abierta: si se va a otra
        // habitacion, la bandeja no puede quedar abierta ahi.
        if (panel.activeSelf && actual != habitacion) pantallas.Cerrar();
        boton.SetActive(actual == habitacion);
    }

    private void Refrescar()
    {
        if (!panel.activeSelf) return;

        int i = 0;
        foreach (ItemDefinition item in inventario.Catalogo.Items)
        {
            if (item == null || item.Categoria != categoria || !inventario.Tiene(item)) continue;
            if (i == slots.Count) slots.Add(CrearSlot());
            InventorySlot slot = slots[i++];
            slot.gameObject.SetActive(true);
            slot.Mostrar(item, inventario.Cantidad(item), EstaActivo(item));
        }
        for (int j = i; j < slots.Count; j++) slots[j].gameObject.SetActive(false);

        vacio.gameObject.SetActive(i == 0);
    }

    private InventorySlot CrearSlot()
    {
        InventorySlot slot = Instantiate(plantilla, contenedor);
        slot.Tocado += AlTocar;
        slot.ArrastreEmpezado += AlEmpezarArrastre;
        slot.Arrastrando += AlArrastrar;
        slot.Soltado += AlSoltar;
        return slot;
    }

    private bool EstaActivo(ItemDefinition item)
    {
        if (item.Categoria == ItemCategory.Ropa) return inventario.Puesto(item);
        if (item.Categoria == ItemCategory.Decoracion) return inventario.Colocado(item);
        return false;
    }

    private void AlTocar(InventorySlot slot)
    {
        ItemDefinition item = slot.Item;
        if (item.Categoria == ItemCategory.Ropa) inventario.AlternarPuesto(item);
        else if (item.Categoria == ItemCategory.Decoracion) inventario.AlternarColocado(item);
        else return;
        SoundPlayer.Reproducir(sonidoPoner);
    }

    private void AlEmpezarArrastre(InventorySlot slot, PointerEventData evento)
    {
        if (fantasma == null) return;
        fantasma.sprite = slot.Item.Icono;
        fantasma.gameObject.SetActive(true);
        fantasma.transform.position = evento.position;
    }

    private void AlArrastrar(PointerEventData evento)
    {
        if (fantasma != null && fantasma.gameObject.activeSelf) fantasma.transform.position = evento.position;
    }

    private void AlSoltar(InventorySlot slot, PointerEventData evento)
    {
        if (fantasma != null) fantasma.gameObject.SetActive(false);
        if (mascota == null) return;
        Comer(slot.Item, Camera.main.ScreenToWorldPoint(evento.position));
    }

    // Publico para probarlo sin dedo. Como FoodItem: solo cuenta si se suelta encima de Poku.
    public bool Comer(ItemDefinition item, Vector2 puntoMundo)
    {
        if (!mascota.Contiene(puntoMundo) || !inventario.Usar(item)) return false;

        mascota.Aplicar(item.Necesidad, item.Cantidad);
        SoundPlayer.Reproducir(sonidoComer);
        if (cara != null) cara.Alegrar(segundosContento);
        if (progreso != null) progreso.Ganar(XpSource.Comer);
        return true;
    }
}
