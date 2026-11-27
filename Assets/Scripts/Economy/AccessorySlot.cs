// ============================================================================
// AccessorySlot.cs
// QUE HACE: lista las partes del cuerpo de Poku donde se puede poner ropa.
//           Solo puede tener un accesorio por ranura a la vez.
// PARA CAMBIAR: agregar una ranura nueva -> sumar un valor aca y despues darle
//               un punto de anclaje en cada PetAppearance (Data/Pet/Apariencia_*).
//               Cada item de ropa elige su ranura en su asset (campo "Ranura").
// ============================================================================
public enum AccessorySlot
{
    Cabeza,     // gorras, coronas, monos
    Cara,       // anteojos
    Cuello,     // bufandas, collares, corbatas
    Espalda     // mochila, alas
}
