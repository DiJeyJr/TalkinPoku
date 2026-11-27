// ============================================================================
// FurnitureSpot.cs
// QUE HACE: los lugares fijos de la Sala principal donde va un mueble. En cada
//           lugar hay un mueble por vez (salvo Cuadro, que tiene dos lugares).
// PARA CAMBIAR: en que lugar va un mueble -> campo "Lugar" de su asset
//               (Data/Items/Item_Mueble_*.asset). La posicion exacta en la sala
//               es "Posicion Inicial" del mismo asset.
// ============================================================================
public enum FurnitureSpot
{
    Ventana,
    Cuadro,         // dos lugares en la pared (ver RoomDecor.lugaresCuadro)
    Alfombra,
    Sillon,
    Mesita,
    LamparaMesa,    // va arriba de la mesita; si no hay mesita, va al piso
    LamparaPie,
    Planta,
    Banqueta
}
