// ============================================================================
// MinigameResult.cs
// QUE HACE: el resultado de una partida de minijuego: puntaje, monedas ganadas,
//           si se completo y si se gano. Cada minijuego lo crea al terminar con
//           new MinigameResult(puntos, monedas, completo, gano).
// ============================================================================
public readonly struct MinigameResult
{
    public int Score { get; }
    // Monedas ganadas (antes del bonus de la Mega).
    public int Coins { get; }
    // false si el jugador abandono a la mitad, para no pagar partidas cortadas.
    public bool Completed { get; }
    // Aparte de Completed: una partida de vuelo termina siempre chocando (completa), pero solo es
    // victoria si llego a la meta. La pantalla de resultado elige el titulo y el sonido con esto.
    public bool Won { get; }

    public MinigameResult(int score, int coins, bool completed, bool won)
    {
        Score = score;
        Coins = coins;
        Completed = completed;
        Won = won;
    }
}
