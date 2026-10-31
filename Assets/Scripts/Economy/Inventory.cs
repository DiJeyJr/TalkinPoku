using System;
using System.Collections.Generic;
using UnityEngine;

/// Lo que el jugador compro: comida para darle, ropa para ponerle a Poku y muebles para la sala.
/// Comprar descuenta de la billetera; usar, poner y colocar avisan con Cambiado para que la UI, Poku
/// y la sala se actualicen.
public class Inventory : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ItemCatalog catalogo;
    [SerializeField] private Wallet billetera;

    public event Action Cambiado;

    // Estatico como el saldo: sobrevive a ir a un minijuego y volver. Al abrir la app lo llena el guardado.
    private static InventoryState estado = new InventoryState();

    public static InventoryState Foto() => estado;

    public static void Restaurar(InventoryState guardado)
    {
        if (guardado == null) return;
        estado = guardado;
        // Una partida vieja puede traer el array con otro largo si se agregan ranuras.
        if (estado.puestos == null || estado.puestos.Length != Enum.GetValues(typeof(AccessorySlot)).Length)
            Array.Resize(ref estado.puestos, Enum.GetValues(typeof(AccessorySlot)).Length);
    }

    public ItemCatalog Catalogo => catalogo;

    public int Cantidad(ItemDefinition item)
    {
        int i = estado.ids.IndexOf(item.Id);
        return i < 0 ? 0 : estado.cantidades[i];
    }

    public bool Tiene(ItemDefinition item) => Cantidad(item) > 0;

    // Ropa y muebles se compran una sola vez.
    public bool PuedeComprar(ItemDefinition item)
    {
        if (item.Precio <= 0 || item.Bloqueado) return false;
        if (!item.SeAcumula && Tiene(item)) return false;
        return billetera.Obtener(item.Moneda) >= item.Precio;
    }

    public bool Comprar(ItemDefinition item)
    {
        if (!PuedeComprar(item) || !billetera.Gastar(item.Moneda, item.Precio)) return false;
        Agregar(item, 1);
        // Una compra no puede depender del proximo autoguardado: si la app se cierra, se pierde lo pagado.
        SaveSystem.Guardar();
        return true;
    }

    // Gasta uno (la comida al comerla).
    public bool Usar(ItemDefinition item)
    {
        if (!Tiene(item)) return false;
        Agregar(item, -1);
        return true;
    }

    public bool Puesto(ItemDefinition item) => estado.puestos[(int)item.Ranura] == item.Id;

    public ItemDefinition PuestoEn(AccessorySlot ranura)
    {
        string id = estado.puestos[(int)ranura];
        return string.IsNullOrEmpty(id) ? null : catalogo.Buscar(id);
    }

    // Ponerse algo saca lo que habia en esa ranura; tocar lo que ya tiene puesto se lo saca.
    public void AlternarPuesto(ItemDefinition item)
    {
        if (!Tiene(item) || item.Categoria != ItemCategory.Ropa) return;
        int r = (int)item.Ranura;
        estado.puestos[r] = estado.puestos[r] == item.Id ? "" : item.Id;
        Cambiado?.Invoke();
    }

    public bool Colocado(ItemDefinition item) => estado.colocados.Contains(item.Id);

    // En el orden en que se pusieron: RoomDecor reparte los cuadros por ese orden.
    public IReadOnlyList<string> Colocados => estado.colocados;

    public void AlternarColocado(ItemDefinition item)
    {
        if (!Tiene(item) || item.Categoria != ItemCategory.Decoracion) return;
        int i = estado.colocados.IndexOf(item.Id);
        if (i >= 0)
        {
            Sacar(i);
        }
        else
        {
            // Un mueble por lugar: el nuevo guarda al que estaba. Con los dos cuadros colgados, sale el
            // mas viejo.
            int lugares = item.Lugar == FurnitureSpot.Cuadro ? 2 : 1;
            List<int> ocupados = new List<int>();
            for (int j = 0; j < estado.colocados.Count; j++)
            {
                ItemDefinition otro = catalogo.Buscar(estado.colocados[j]);
                if (otro != null && otro.Lugar == item.Lugar) ocupados.Add(j);
            }
            for (int j = ocupados.Count - lugares; j >= 0; j--) Sacar(ocupados[j]);

            estado.colocados.Add(item.Id);
            // Las posiciones ya no se usan (los muebles van a su lugar fijo), pero el guardado las espera.
            estado.posiciones.Add(item.PosicionInicial);
        }
        Cambiado?.Invoke();
    }

    private void Sacar(int i)
    {
        estado.colocados.RemoveAt(i);
        if (i < estado.posiciones.Count) estado.posiciones.RemoveAt(i);
    }

    // Para el panel de admin.
    public void Vaciar()
    {
        estado = new InventoryState();
        Cambiado?.Invoke();
    }

    private void Agregar(ItemDefinition item, int cuantos)
    {
        int i = estado.ids.IndexOf(item.Id);
        if (i < 0)
        {
            estado.ids.Add(item.Id);
            estado.cantidades.Add(0);
            i = estado.ids.Count - 1;
        }

        estado.cantidades[i] = Mathf.Max(0, estado.cantidades[i] + cuantos);
        if (estado.cantidades[i] == 0)
        {
            estado.ids.RemoveAt(i);
            estado.cantidades.RemoveAt(i);
        }
        Cambiado?.Invoke();
    }
}
