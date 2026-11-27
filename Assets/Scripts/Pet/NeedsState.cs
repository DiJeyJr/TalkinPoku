using System;

// ============================================================================
// NeedsState.cs
// QUE HACE: la "foto" de las necesidades que se guarda en la partida y que se
//           pasa entre la casa y los minijuegos.
// ============================================================================
[Serializable]
public class NeedsState
{
    // Hambre, higiene, diversion y energia (en el orden de NeedType).
    public float[] valores;
    public bool dormido;
    // UTC en ISO-8601 ("o"), para poder calcular cuanto tiempo paso sin depender de la zona horaria.
    public string momentoUtc;
}
