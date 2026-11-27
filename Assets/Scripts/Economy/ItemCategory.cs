// ============================================================================
// ItemCategory.cs
// QUE HACE: los tipos de item. Define en que pestana de la tienda aparece y
//           como se usa (la comida se come, la ropa se pone, los muebles se colocan).
// PARA CAMBIAR: la categoria de un item -> campo "Categoria" en su asset.
// ============================================================================
public enum ItemCategory
{
    Comida,      // se acumula y se gasta al comerla
    Ropa,        // se compra una vez y se pone en Poku
    Decoracion   // muebles: se compran una vez y se colocan en la sala
}
