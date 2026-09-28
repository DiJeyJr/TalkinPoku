using System;
using System.Collections.Generic;

/// Lo que se escribe a disco. Solo campos publicos, arrays y listas: es lo que entiende JsonUtility
/// (no serializa Dictionary ni DateTime).
[Serializable]
public class SaveData
{
    // Se sube cuando cambia el formato, para saber al leer una partida vieja que hay que convertirla.
    public const int VersionActual = 1;

    public int version;
    // Trae el momento en que se saco la foto: al abrir se descuenta el tiempo que paso desde ahi.
    public NeedsState necesidades;
    // Monedas y gemas, en el orden de CurrencyType.
    public int[] saldo;
    // Por id y no por referencia al asset: renombrar o mover una receta no rompe la partida.
    public List<string> recetas = new List<string>();
}
