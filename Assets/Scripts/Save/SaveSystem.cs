using System;
using System.IO;
using UnityEngine;

/// Lee y escribe la partida en un JSON. Las necesidades, la billetera y las recetas viven cada una en
/// su sistema: aca solo se juntan para escribirlas y se reparten al abrir el juego.
public static class SaveSystem
{
    private static string Ruta => Path.Combine(Application.persistentDataPath, "partida.json");
    private static string RutaTemporal => Path.Combine(Application.persistentDataPath, "partida.tmp");
    private static string RutaRespaldo => Path.Combine(Application.persistentDataPath, "partida.bak");

    private static SaveData datos = new SaveData();

    // Unity lo llama solo al abrir el juego, antes de cargar la primera escena: la partida ya esta en
    // memoria cuando corren los Awake, abra la escena que abra.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AlAbrir()
    {
        Cargar();

        // El que decide cuando guardar vive fuera de las escenas, asi sigue andando en los minijuegos.
        GameObject objeto = new GameObject("AutoSave");
        UnityEngine.Object.DontDestroyOnLoad(objeto);
        objeto.AddComponent<AutoSave>();
    }

    public static void Cargar()
    {
        // Si la partida quedo cortada o rota se usa el respaldo, que es la anterior.
        SaveData leidos = Leer(Ruta) ?? Leer(RutaRespaldo);
        // Primera vez que se abre: cada sistema arranca con sus valores iniciales.
        if (leidos == null) return;

        datos = leidos;
        NeedsSystem.Restaurar(datos.necesidades);
        Wallet.Restaurar(datos.saldo);
        RecipeBook.Restaurar(datos.recetas);
    }

    public static void Guardar()
    {
        // null = ese sistema todavia no arranco en esta sesion. Se deja lo que se habia leido.
        NeedsState necesidades = NeedsSystem.Foto();
        if (necesidades != null) datos.necesidades = necesidades;

        int[] saldo = Wallet.Foto();
        if (saldo != null) datos.saldo = saldo;

        datos.recetas = RecipeBook.Foto();
        datos.version = SaveData.VersionActual;

        try
        {
            // Primero se escribe entero en un temporal y despues se cambian nombres, que no puede
            // quedar a medias. Si el proceso muere en el medio queda la partida anterior como respaldo,
            // nunca un archivo cortado.
            File.WriteAllText(RutaTemporal, JsonUtility.ToJson(datos));
            if (File.Exists(Ruta))
            {
                File.Delete(RutaRespaldo);
                File.Move(Ruta, RutaRespaldo);
            }
            File.Move(RutaTemporal, Ruta);
        }
        catch (Exception e)
        {
            Debug.LogWarning("No se pudo guardar la partida: " + e.Message);
        }
    }

    private static SaveData Leer(string archivo)
    {
        if (!File.Exists(archivo)) return null;

        try
        {
            SaveData leidos = JsonUtility.FromJson<SaveData>(File.ReadAllText(archivo));
            // Sin version no es una partida: archivo vacio o cortado.
            return leidos != null && leidos.version > 0 ? leidos : null;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Partida ilegible en " + archivo + ": " + e.Message);
            return null;
        }
    }
}
