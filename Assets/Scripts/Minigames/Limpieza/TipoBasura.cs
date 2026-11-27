// ============================================================================
// TipoBasura.cs  (minijuego "Limpieza")
// QUE HACE: los tipos de basura; cada uno se junta con un gesto distinto.
// ============================================================================
public enum TipoBasura
{
    Suelta,     // tap o arrastrarla al cesto
    Pegada,     // mantener apretado (hold)
    EnArbol     // sacudir el celular para que caiga
}
