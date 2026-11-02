using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryState
{
    // Lo que tiene y cuantos (la comida se junta; ropa y muebles van con 1).
    public List<string> ids = new List<string>();
    public List<int> cantidades = new List<int>();
    // Lo que tiene puesto Poku, uno por ranura en el orden de AccessorySlot. "" = nada.
    public string[] puestos = new string[Enum.GetValues(typeof(AccessorySlot)).Length];
    // Los muebles que estan en la sala.
    public List<string> colocados = new List<string>();
}
