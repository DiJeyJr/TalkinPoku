/// Como termino una partida. Es un struct de solo lectura porque es un dato que se pasa, no algo
/// que alguien tenga que modificar despues.
public readonly struct MinigameResult
{
    public int Score { get; }
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
