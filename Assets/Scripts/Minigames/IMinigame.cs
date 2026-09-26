using System;

/// Lo que ve MinigameFlow de cualquier minijuego: se arranca y avisa cuando termino. Asi el flujo
/// de entrar, mostrar el resultado y volver a la casa se escribe una vez para los seis.
public interface IMinigame
{
    event Action<MinigameResult> Finished;
    void StartGame();
}
