using System.Collections.Generic;

// ============================================================================
// RegistroIds.cs
// QUE HACE: guarda una lista de ids "ya conseguidos". Hay dos registros fijos:
//           RegistroIds.Recetas (recetas descubiertas en Cocina) y
//           RegistroIds.EscenariosLimpios (escenarios terminados en Limpieza).
// DONDE ESTA: en ningun objeto (instancias estaticas). Lo guarda SaveSystem.
// USO: if (RegistroIds.Recetas.Registrar(id)) { ...primera vez... }
// ============================================================================

// Ids que el jugador ya consiguio: las recetas que descubrio y los escenarios que dejo limpios.
// Estaticos como la Despensa: sobreviven a ir y volver de la casa, y los guarda SaveSystem.
public class RegistroIds
{
    public static readonly RegistroIds Recetas = new RegistroIds();
    public static readonly RegistroIds EscenariosLimpios = new RegistroIds();

    // HashSet: no permite repetidos y busca rapido.
    private readonly HashSet<string> ids = new HashSet<string>();

    // Para el guardado.
    // Lista y no HashSet porque es lo que JsonUtility sabe escribir.
    public List<string> Foto()
    {
        return new List<string>(ids);
    }

    // Al abrir el juego: recupera lo guardado.
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
