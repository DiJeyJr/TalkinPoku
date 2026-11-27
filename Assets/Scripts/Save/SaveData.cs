using System;
using System.Collections.Generic;

// ============================================================================
// SaveData.cs
// QUE HACE: es el "molde" de la partida guardada. Cada campo publico de esta
//           clase se escribe en el archivo partida.json.
// DONDE ESTA: no va en ningun objeto; lo usa SaveSystem.
// PARA GUARDAR ALGO NUEVO (por ejemplo, vidas o un puntaje record):
//   1) Agregar un campo publico aca (ej: public int vidas;).
//   2) En SaveSystem.Guardar() copiar el valor actual a datos.vidas.
//   3) En SaveSystem.Cargar() devolverlo al sistema que lo usa.
//   (Opcional) subir VersionActual y agregar un paso en SaveSystem.Migrar().
// Archivo en el celular/PC: Application.persistentDataPath/partida.json
// (en Windows: AppData/LocalLow/Grupo 12/Talkin Poku/).
// ============================================================================

// Lo que se escribe a disco. Solo campos publicos, arrays y listas: es lo que entiende JsonUtility
// (no serializa Dictionary ni DateTime).
[Serializable]
public class SaveData
{
    // Se sube cuando cambia el formato, para saber al leer una partida vieja que hay que convertirla.
    // 1: necesidades, saldo y recetas. 2: se suman la progresion, los accesorios y muebles, y los
    // ingredientes y platos. 3: se suman los escenarios limpiados del minijuego de limpieza.
    public const int VersionActual = 3;

    public int version;
    // Hambre, higiene, diversion, energia, si dormia y la hora en que se guardo.
    // Trae el momento en que se saco la foto: al abrir se descuenta el tiempo que paso desde ahi.
    public NeedsState necesidades;
    // Monedas y gemas, en el orden de CurrencyType.
    public int[] saldo;
    // Recetas de Cocina ya descubiertas.
    // Por id y no por referencia al asset: renombrar o mover una receta no rompe la partida.
    public List<string> recetas = new List<string>();
    // Nivel y experiencia de Poku.
    // Una partida de antes de la progresion los trae en 0: Progression.Restaurar la deja en el nivel 1.
    public int nivel;
    public int xp;
    // Barra de la Mega y hasta cuando dura si esta activa.
    // megaHasta en ticks UTC: JsonUtility no guarda DateTime. 0 = la Mega no estaba activa.
    public int energiaMega;
    public long megaHasta;
    // Si ya vio el cartel de felicitaciones de la primera Mega.
    // Una partida vieja lo trae en false: si ya habia usado la Mega, ve el cartel una vez mas.
    public bool megaPresentada;
    // Comida, ropa y muebles comprados, que tiene puesto y que muebles coloco.
    public InventoryState inventario;
    // Ingredientes y platos de la despensa (cocina de la casa).
    public List<ItemCantidad> ingredientes = new List<ItemCantidad>();
    public List<ItemCantidad> platos = new List<ItemCantidad>();
    // Escenarios del minijuego Limpieza que ya limpio.
    // Por id, igual que las recetas.
    public List<string> escenariosLimpios = new List<string>();
}

// Un par "id + cantidad", para guardar listas de cosas que se acumulan.
[Serializable]
public class ItemCantidad
{
    public string id;
    public int cantidad;
}
