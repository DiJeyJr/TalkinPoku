using UnityEngine;

// ============================================================================
// AutoSave.cs
// QUE HACE: decide CUANDO se guarda la partida: cada tantos segundos, al pasar
//           la app a segundo plano y al cerrarla. Lo que se guarda lo arma
//           SaveSystem.
// DONDE ESTA: lo crea SaveSystem solo al abrir el juego (objeto "AutoSave" que
//             no se destruye al cambiar de escena).
// PARA CAMBIAR: cada cuantos segundos se guarda -> segundosEntreGuardados.
// ============================================================================

// Decide cuando guardar. Lo crea SaveSystem al abrir el juego y sobrevive a los cambios de escena.
// En Android el sistema puede cerrar la app sin llamar a nada, por eso no alcanza con guardar al
// salir: tambien se guarda cada tantos segundos.
public class AutoSave : MonoBehaviour
{
    [Header("Umbrales")]
    // [AJUSTE] Segundos entre guardados automaticos.
    [SerializeField] private float segundosEntreGuardados = 30f;

    private float proximoGuardado;

    private void Start()
    {
        proximoGuardado = Time.unscaledTime + segundosEntreGuardados;
    }

    // Cuando llega la hora del proximo guardado, guarda y programa el siguiente.
    private void Update()
    {
        // unscaledTime: con el juego pausado por timeScale tambien tiene que guardar.
        if (Time.unscaledTime < proximoGuardado) return;

        proximoGuardado = Time.unscaledTime + segundosEntreGuardados;
        SaveSystem.Guardar();
    }

    // Pasar a segundo plano es el ultimo aviso confiable en el celular.
    private void OnApplicationPause(bool pausada)
    {
        if (pausada) SaveSystem.Guardar();
    }

    // Al cerrar la app (en el Editor, al salir de Play).
    private void OnApplicationQuit()
    {
        SaveSystem.Guardar();
    }
}
