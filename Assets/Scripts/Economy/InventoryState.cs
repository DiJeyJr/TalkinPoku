using System;
using System.Collections.Generic;
using UnityEngine;

/// El inventario como lo entiende JsonUtility: listas paralelas en vez de diccionarios.
[Serializable]
public class InventoryState
{
    // Lo que tiene y cuantos (la comida se junta; ropa y muebles van con 1).
    public List<string> ids = new List<string>();
    public List<int> cantidades = new List<int>();
    // Lo que tiene puesto Poku, uno por ranura en el orden de AccessorySlot. "" = nada.
    public string[] puestos = new string[4];
    // Los muebles que estan en la sala y donde los dejo el jugador.
    public List<string> colocados = new List<string>();
    public List<Vector2> posiciones = new List<Vector2>();
}
