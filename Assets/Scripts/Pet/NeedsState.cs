using System;

[Serializable]
public class NeedsState
{
    public float[] valores;
    public bool dormido;
    // UTC en ISO-8601 ("o"), para poder calcular cuanto tiempo paso sin depender de la zona horaria.
    public string momentoUtc;
}
