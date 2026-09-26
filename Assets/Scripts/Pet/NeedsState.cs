using System;

/// Foto de las necesidades en un momento dado. Hoy sirve para pasar de una escena a otra; cuando
/// exista el guardado, es lo mismo que se va a escribir a disco (por eso campos publicos y la
/// fecha como texto: JsonUtility no serializa DateTime).
[Serializable]
public class NeedsState
{
    public float[] valores;
    public bool dormido;
    // UTC en ISO-8601 ("o"), para poder calcular cuanto tiempo paso sin depender de la zona horaria.
    public string momentoUtc;
}
