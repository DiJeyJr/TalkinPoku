using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ============================================================================
// SaveSystem.cs
// QUE HACE: lee y escribe la partida (partida.json). Al abrir el juego reparte
//           los datos guardados a cada sistema (necesidades, monedas, nivel,
//           inventario...). Al guardar junta los datos de todos y los escribe.
// DONDE ESTA: en ningun objeto (clase estatica). Unity lo ejecuta solo al abrir
//             el juego y crea el AutoSave.
// FLUJO: AlAbrir -> Cargar -> cada sistema recibe lo suyo con Restaurar(...).
//        AutoSave -> Guardar -> se pide a cada sistema su Foto() y se escribe.
// PARA GUARDAR ALGO NUEVO: ver los pasos en el encabezado de SaveData.cs.
// PARA BORRAR LA PARTIDA (empezar de cero): borrar partida.json y partida.bak
//   de Application.persistentDataPath (en Windows: AppData/LocalLow/Grupo 12/
//   Talkin Poku/) con el juego cerrado. El panel de admin solo reinicia el
//   nivel y vacia el inventario, no borra todo.
// ============================================================================

// Lee y escribe la partida en un JSON. Las necesidades, la billetera, las recetas y el nivel viven cada uno en
// su sistema: aca solo se juntan para escribirlas y se reparten al abrir el juego.
public static class SaveSystem
{
    // Archivos: la partida, un temporal mientras se escribe y el respaldo (la partida anterior).
    private static string Ruta => Path.Combine(Application.persistentDataPath, "partida.json");
    private static string RutaTemporal => Path.Combine(Application.persistentDataPath, "partida.tmp");
    private static string RutaRespaldo => Path.Combine(Application.persistentDataPath, "partida.bak");

    // La partida en memoria.
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

    // Lee el archivo y le devuelve a cada sistema sus datos guardados.
    public static void Cargar()
    {
        // Si la partida quedo cortada o rota se usa el respaldo, que es la anterior.
        SaveData leidos = Leer(Ruta) ?? Leer(RutaRespaldo);
        // Primera vez que se abre: cada sistema arranca con sus valores iniciales.
        if (leidos == null) return;

        datos = Migrar(leidos);
        // Un Restaurar por sistema. Si se agrega algo nuevo a SaveData, se reparte aca.
        NeedsSystem.Restaurar(datos.necesidades);
        Wallet.Restaurar(datos.saldo);
        RegistroIds.Recetas.Restaurar(datos.recetas);
        Progression.Restaurar(datos.nivel, datos.xp);
        MegaEvolution.Restaurar(datos.energiaMega, datos.megaHasta, datos.megaPresentada);
        Inventory.Restaurar(datos.inventario);
        Despensa.Ingredientes.Restaurar(datos.ingredientes);
        Despensa.Platos.Restaurar(datos.platos);
        RegistroIds.EscenariosLimpios.Restaurar(datos.escenariosLimpios);
    }

    // Un paso por version, en cascada, sin borrar nunca lo que no se entiende: una partida vieja se
    // convierte en vez de perderse.
    private static SaveData Migrar(SaveData viejos)
    {
        if (viejos.version < 2)
        {
            // La 1 no tenia despensa. Nivel e inventario de accesorios ya se arreglan solos al restaurar.
            if (viejos.ingredientes == null) viejos.ingredientes = new List<ItemCantidad>();
            if (viejos.platos == null) viejos.platos = new List<ItemCantidad>();
        }

        if (viejos.version < 3)
        {
            // La 2 no tenia el minijuego de limpieza: arranca sin escenarios limpios.
            if (viejos.escenariosLimpios == null) viejos.escenariosLimpios = new List<string>();
        }

        viejos.version = SaveData.VersionActual;
        return viejos;
    }

    // Junta el estado actual de todos los sistemas y lo escribe en el archivo.
    // Se puede llamar desde cualquier lado con SaveSystem.Guardar() para forzar un guardado.
    public static void Guardar()
    {
        // null = ese sistema todavia no arranco en esta sesion. Se deja lo que se habia leido.
        NeedsState necesidades = NeedsSystem.Foto();
        if (necesidades != null) datos.necesidades = necesidades;

        int[] saldo = Wallet.Foto();
        if (saldo != null) datos.saldo = saldo;

        // Una linea por dato. Si se agrega algo nuevo a SaveData, se junta aca.
        datos.recetas = RegistroIds.Recetas.Foto();
        datos.nivel = Progression.NivelActual;
        datos.xp = Progression.XpActual;
        datos.energiaMega = MegaEvolution.EnergiaActual;
        datos.megaHasta = MegaEvolution.MegaHasta;
        datos.megaPresentada = MegaEvolution.Presentada;
        datos.inventario = Inventory.Foto();
        datos.ingredientes = Despensa.Ingredientes.Foto();
        datos.platos = Despensa.Platos.Foto();
        datos.escenariosLimpios = RegistroIds.EscenariosLimpios.Foto();
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

    // Lee un archivo y lo convierte de JSON a SaveData. Si no existe o esta roto devuelve null.
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
