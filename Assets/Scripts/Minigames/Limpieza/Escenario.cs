using System;
using System.Collections.Generic;
using UnityEngine;

// ============================================================================
// Escenario.cs  (minijuego "Limpieza")
// QUE HACE: describe un lugar para limpiar: nombre, fondo sucio y limpio, donde
//           esta cada basura (y de que tipo) y cuantas monedas paga.
// DONDE ESTA: un asset por escenario en Data/Escenarios (Playa, Parque, Bosque).
// PARA CAMBIAR: monedas -> "Monedas". Basuras -> lista "Basuras" (sprite,
//               posicion y tipo de cada una).
// PARA UNO NUEVO: Create > TalkinPoku > Escenario y sumarlo a la lista
//                 "Escenarios" de LimpiezaGame.
// ============================================================================

// Un lugar para limpiar: sus dos fondos, donde esta cada basura y cuanto paga. Para sumar un
// escenario se crea otro asset y se agrega a la lista de LimpiezaGame, sin tocar codigo.
[CreateAssetMenu(menuName = "TalkinPoku/Escenario", fileName = "Escenario_Nuevo")]
public class Escenario : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string nombre;
    [SerializeField] private Sprite fondoSucio;
    [SerializeField] private Sprite fondoLimpio;
    // [AJUSTE] Basuras del escenario.
    [SerializeField] private BasuraEnEscenario[] basuras;
    // [AJUSTE] Monedas por limpiarlo entero.
    [SerializeField] private int monedas = 30;

    public string Id => id;
    public string Nombre => nombre;
    public Sprite FondoSucio => fondoSucio;
    public Sprite FondoLimpio => fondoLimpio;
    public IReadOnlyList<BasuraEnEscenario> Basuras => basuras;
    public int Monedas => monedas;
}

// Posiciones fijas y no al azar: asi cada escenario se arma a mano (la lata en la arena, la bolsa
// enganchada en el arbusto) y siempre se ve igual.
[Serializable]
public class BasuraEnEscenario
{
    public Sprite sprite;
    public Vector2 posicion;
    public TipoBasura tipo;
}
