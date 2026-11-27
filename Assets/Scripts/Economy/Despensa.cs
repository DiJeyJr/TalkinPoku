using System;
using System.Collections.Generic;

// ============================================================================
// Despensa.cs
// QUE HACE: cuenta cuantos ingredientes y cuantos platos cocinados tiene el
//           jugador. Hay dos despensas fijas: Despensa.Ingredientes y
//           Despensa.Platos.
// DONDE ESTA: en ningun objeto (las dos instancias son estaticas). La usan la
//             tienda de ingredientes, el minijuego Cocina y la Mesada.
// USO: Despensa.Ingredientes.Sumar("huevo", 2);  Despensa.Platos.Gastar("flan");
// AVISA: el evento Cambio(id, cantidadNueva).
// ============================================================================

// Cuantos tiene el jugador de cada cosa, por id. Hay uno para los ingredientes que se compran en
// la tienda y otro para los platos que se cocinan. Estaticos como la billetera: sobreviven a ir y
// volver de un minijuego, y los guarda SaveSystem.
public class Despensa
{
    public static readonly Despensa Ingredientes = new Despensa();
    public static readonly Despensa Platos = new Despensa();

    // id y cantidad nueva. Los contadores de la tienda, el estante y la mesada se enteran por aca.
    public event Action<string, int> Cambio;

    // id -> cantidad.
    private readonly Dictionary<string, int> cantidades = new Dictionary<string, int>();

    // Suma de todo lo que hay en esta despensa.
    public int Total
    {
        get
        {
            int total = 0;
            foreach (int cantidad in cantidades.Values) total += cantidad;
            return total;
        }
    }

    // Cuantos hay de un id (0 si no hay ninguno).
    public int Cantidad(string id)
    {
        return cantidades.TryGetValue(id, out int cantidad) ? cantidad : 0;
    }

    // Agrega unidades de un id.
    public void Sumar(string id, int cantidad = 1)
    {
        if (cantidad <= 0) return;
        cantidades[id] = Cantidad(id) + cantidad;
        Cambio?.Invoke(id, cantidades[id]);
    }

    // Saca unidades de un id.
    // false si no hay suficientes, y no se descuenta nada.
    public bool Gastar(string id, int cantidad = 1)
    {
        int tiene = Cantidad(id);
        if (cantidad <= 0 || tiene < cantidad) return false;
        cantidades[id] = tiene - cantidad;
        Cambio?.Invoke(id, cantidades[id]);
        return true;
    }

    // Para el guardado.
    // Lista de pares y no el Dictionary: JsonUtility no sabe escribir diccionarios.
    public List<ItemCantidad> Foto()
    {
        List<ItemCantidad> foto = new List<ItemCantidad>();
        foreach (KeyValuePair<string, int> par in cantidades)
        {
            if (par.Value > 0) foto.Add(new ItemCantidad { id = par.Key, cantidad = par.Value });
        }
        return foto;
    }

    // Al abrir el juego: vuelve a llenar la despensa con lo guardado.
    public void Restaurar(List<ItemCantidad> guardado)
    {
        if (guardado == null) return;
        cantidades.Clear();
        foreach (ItemCantidad item in guardado)
        {
            if (!string.IsNullOrEmpty(item.id) && item.cantidad > 0) cantidades[item.id] = item.cantidad;
        }
    }
}
