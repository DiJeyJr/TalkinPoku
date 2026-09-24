using UnityEngine;

/// Un objeto del juego (por ahora, comida). El id es lo que va a ir al guardado: si se renombra
/// el asset, las partidas guardadas siguen funcionando.
[CreateAssetMenu(menuName = "TalkinPoku/Item", fileName = "Item_")]
public class ItemDefinition : ScriptableObject
{
    [Header("Identidad")]
    [SerializeField] private string id;
    [SerializeField] private string nombre;
    [SerializeField] private Sprite icono;

    [Header("Efecto")]
    [SerializeField] private NeedType necesidad = NeedType.Hambre;
    [SerializeField] private float cantidad = 20f;

    public string Id => id;
    public string Nombre => nombre;
    public Sprite Icono => icono;
    public NeedType Necesidad => necesidad;
    public float Cantidad => cantidad;
}
