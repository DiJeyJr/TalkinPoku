using System.Collections.Generic;

// Ids que el jugador ya consiguio: las recetas que descubrio y los escenarios que dejo limpios.
// Estaticos como la Despensa: sobreviven a ir y volver de la casa, y los guarda SaveSystem.
public class RegistroIds
{
    public static readonly RegistroIds Recetas = new RegistroIds();
    public static readonly RegistroIds EscenariosLimpios = new RegistroIds();

    private readonly HashSet<string> ids = new HashSet<string>();

    // Lista y no HashSet porque es lo que JsonUtility sabe escribir.
    public List<string> Foto()
    {
        return new List<string>(ids);
    }

    public void Restaurar(List<string> guardados)
    {
        if (guardados == null) return;
        ids.Clear();
        ids.UnionWith(guardados);
    }

    public bool Contiene(string id)
    {
        return ids.Contains(id);
    }

    // true solo la primera vez: HashSet.Add ya devuelve si el id era nuevo.
    public bool Registrar(string id)
    {
        return ids.Add(id);
    }
}
