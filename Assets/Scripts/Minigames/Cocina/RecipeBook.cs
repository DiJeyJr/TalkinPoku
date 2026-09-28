using System.Collections.Generic;

/// Las recetas que el jugador ya descubrio, por id. Estatico como el saldo de la billetera, asi
/// sobrevive a ir y volver de la casa. Al abrir la app lo llena el guardado.
public static class RecipeBook
{
    private static readonly HashSet<string> descubiertas = new HashSet<string>();

    public static int Cantidad => descubiertas.Count;

    // Lista y no HashSet porque es lo que JsonUtility sabe escribir.
    public static List<string> Foto()
    {
        return new List<string>(descubiertas);
    }

    public static void Restaurar(List<string> ids)
    {
        if (ids == null) return;
        descubiertas.Clear();
        descubiertas.UnionWith(ids);
    }

    public static bool EstaDescubierta(string id)
    {
        return descubiertas.Contains(id);
    }

    // true solo la primera vez: HashSet.Add ya devuelve si el elemento era nuevo.
    public static bool Registrar(string id)
    {
        return descubiertas.Add(id);
    }
}
