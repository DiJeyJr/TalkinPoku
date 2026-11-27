// ============================================================================
// NeedType.cs
// QUE HACE: las necesidades de Poku. Se usan en todo el juego para decir que
//           necesidad sube o baja (ej: NeedType.Hambre).
// ============================================================================

// Felicidad va ultima a proposito: las otras cuatro se guardan como indice de un array y
// felicidad se calcula a partir de ellas.
public enum NeedType
{
    Hambre,     // sube con la comida
    Higiene,    // sube al limpiar las manchas con la esponja
    Diversion,  // sube con la pelota, caricias y minijuegos
    Energia,    // sube durmiendo en la cama
    Felicidad   // promedio de las otras cuatro (no se sube directo)
}
