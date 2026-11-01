using System;
using System.Collections.Generic;

/// Lo que se escribe a disco. Solo campos publicos, arrays y listas: es lo que entiende JsonUtility
/// (no serializa Dictionary ni DateTime).
[Serializable]
public class SaveData
{
    // Se sube cuando cambia el formato, para saber al leer una partida vieja que hay que convertirla.
    // 1: necesidades, saldo y recetas. 2: se suman la progresion, los accesorios y muebles, y los
    // ingredientes y platos.
    public const int VersionActual = 2;

    public int version;
    // Trae el momento en que se saco la foto: al abrir se descuenta el tiempo que paso desde ahi.
    public NeedsState necesidades;
    // Monedas y gemas, en el orden de CurrencyType.
    public int[] saldo;
    // Por id y no por referencia al asset: renombrar o mover una receta no rompe la partida.
    public List<string> recetas = new List<string>();
    // Una partida de antes de la progresion los trae en 0: Progression.Restaurar la deja en el nivel 1.
    public int nivel;
    public int xp;
    // megaHasta en ticks UTC: JsonUtility no guarda DateTime. 0 = la Mega no estaba activa.
    public int energiaMega;
    public long megaHasta;
    // Una partida vieja lo trae en false: si ya habia usado la Mega, ve el cartel una vez mas.
    public bool megaPresentada;
    public InventoryState inventario;
    public List<ItemCantidad> ingredientes = new List<ItemCantidad>();
    public List<ItemCantidad> platos = new List<ItemCantidad>();
}

[Serializable]
public class ItemCantidad
{
    public string id;
    public int cantidad;
}
