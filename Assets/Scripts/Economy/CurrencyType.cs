// ============================================================================
// CurrencyType.cs
// QUE HACE: los tipos de moneda del juego. El orden importa: es el indice en el
//           array de saldo de Wallet (0 = Monedas, 1 = Gemas).
// PARA CAMBIAR: con que se paga un item -> campo "Moneda" en su asset
//               (Data/Items/Item_*.asset).
// ============================================================================
public enum CurrencyType
{
    Monedas,    // se ganan en los minijuegos
    Gemas       // moneda premium (packs de la tienda)
}
