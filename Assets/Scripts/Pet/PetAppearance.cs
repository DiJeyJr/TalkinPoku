using UnityEngine;

// ============================================================================
// PetAppearance.cs
// QUE HACE: guarda todas las caras (sprites) de UNA evolucion de la mascota,
//           donde va la ropa en ese cuerpo y que tan grande es.
// DONDE ESTA: un asset por evolucion en Data/Pet: Apariencia_Poku,
//             Apariencia_Pokucha, Apariencia_Pokuchan y Apariencia_MegaPokuchan.
// PARA CAMBIAR (Inspector del asset de esa evolucion):
//   - un sprite de cara -> el campo de esa expresion (Normal, Contento, ...)
//   - tamano de la mascota en esa etapa -> "Escala"
//   - donde va la ropa -> "Ancla Cabeza/Cara/Cuello/Espalda" y "Escala Accesorios"
// ============================================================================

// Las caras de una etapa de la mascota (Poku, Pokucha, Pokuchan). PetMoodController elige cual
// mostrar; PetEvolution cambia de apariencia al evolucionar.
[CreateAssetMenu(menuName = "TalkinPoku/Apariencia de mascota", fileName = "Apariencia_")]
public class PetAppearance : ScriptableObject
{
    [Header("Expresiones")]
    // [AJUSTE] Un sprite por estado de animo.
    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite contento;
    [SerializeField] private Sprite conHambre;
    [SerializeField] private Sprite sucio;
    [SerializeField] private Sprite aburrido;
    [SerializeField] private Sprite cansado;
    [SerializeField] private Sprite dormido;
    [SerializeField] private Sprite acariciado;

    [Header("Minijuegos")]
    // Sprite que usa el minijuego Vuelo.
    [SerializeField] private Sprite avion;

    [Header("Accesorios")]
    // [AJUSTE] Puntos de anclaje de la ropa.
    // Donde va cada ranura, como fraccion del sprite (0,0 abajo a la izquierda, 1,1 arriba a la
    // derecha), medido sobre la cara normal. Las otras caras tienen la cabeza en el mismo lugar.
    [SerializeField] private Vector2 anclaCabeza = new Vector2(0.45f, 0.7f);
    [SerializeField] private Vector2 anclaCara = new Vector2(0.45f, 0.5f);
    [SerializeField] private Vector2 anclaCuello = new Vector2(0.45f, 0.4f);
    [SerializeField] private Vector2 anclaEspalda = new Vector2(0.45f, 0.35f);
    // [AJUSTE] Tamano general de la ropa en esta etapa.
    // Las evoluciones tienen la cabeza mas chica en proporcion al cuerpo.
    [SerializeField] private float escalaAccesorios = 1f;

    [Header("Tamano")]
    // [AJUSTE] Tamano de la mascota en esta etapa.
    // Cada evolucion es un poco mas grande que la anterior.
    [SerializeField] private float escala = 1f;

    // Una etapa a medio dibujar usa la cara normal en las que le faltan, asi no aparecen las de Poku
    // mezcladas. Dormido y Acariciado no: PetMoodController ya sigue de largo si no estan.
    public Sprite Normal => normal;
    public Sprite Contento => contento != null ? contento : normal;
    public Sprite ConHambre => conHambre != null ? conHambre : normal;
    public Sprite Sucio => sucio != null ? sucio : normal;
    public Sprite Aburrido => aburrido != null ? aburrido : normal;
    public Sprite Cansado => cansado != null ? cansado : normal;
    public Sprite Dormido => dormido;
    public Sprite Acariciado => acariciado;
    public Sprite Avion => avion;
    public float Escala => escala;
    public float EscalaAccesorios => escalaAccesorios;

    // Devuelve el punto de anclaje de una ranura.
    public Vector2 Ancla(AccessorySlot ranura)
    {
        switch (ranura)
        {
            case AccessorySlot.Cabeza: return anclaCabeza;
            case AccessorySlot.Cara: return anclaCara;
            case AccessorySlot.Cuello: return anclaCuello;
            default: return anclaEspalda;
        }
    }
}
