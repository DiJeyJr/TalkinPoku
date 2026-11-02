using System;

// Foto de las necesidades: pasa de una escena a otra y es lo que se escribe en el guardado. La fecha
// va como texto porque JsonUtility no serializa DateTime.
[Serializable]
public class NeedsState
{
    public float[] valores;
    public bool dormido;
    // UTC en ISO-8601 ("o"), para poder calcular cuanto tiempo paso sin depender de la zona horaria.
    public string momentoUtc;
}
