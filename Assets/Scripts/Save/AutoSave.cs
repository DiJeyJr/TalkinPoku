using UnityEngine;

/// Decide cuando guardar. Lo crea SaveSystem al abrir el juego y sobrevive a los cambios de escena.
/// En Android el sistema puede cerrar la app sin llamar a nada, por eso no alcanza con guardar al
/// salir: tambien se guarda cada tantos segundos.
public class AutoSave : MonoBehaviour
{
    [Header("Umbrales")]
    [SerializeField] private float segundosEntreGuardados = 30f;

    private float proximoGuardado;

    private void Start()
    {
        proximoGuardado = Time.unscaledTime + segundosEntreGuardados;
    }

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

    private void OnApplicationQuit()
    {
        SaveSystem.Guardar();
    }
}
