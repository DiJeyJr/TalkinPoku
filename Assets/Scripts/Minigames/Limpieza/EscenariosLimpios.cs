using System.Collections.Generic;

/// Los escenarios que el jugador ya dejo limpios, por id. Estatico como RecipeBook, asi sobrevive
/// a ir y volver de la casa. Al abrir la app lo llena el guardado.
public static class EscenariosLimpios
{
    private static readonly HashSet<string> limpios = new HashSet<string>();

    // Lista y no HashSet porque es lo que JsonUtility sabe escribir.
    public static List<string> Foto()
    {
        return new List<string>(limpios);
    }

    public static void Restaurar(List<string> ids)
    {
        if (ids == null) return;
        limpios.Clear();
        limpios.UnionWith(ids);
    }

    public static bool EstaLimpio(string id)
    {
        return limpios.Contains(id);
    }

    // true solo la primera vez, para saber si se desbloqueo algo nuevo.
    public static bool Registrar(string id)
    {
        return limpios.Add(id);
    }
}
