using UnityEngine;

/// Las caras de una etapa de la mascota (Poku, Pokucha, Pokuchan). PetMoodController elige cual
/// mostrar; PetEvolution cambia de apariencia al evolucionar.
[CreateAssetMenu(menuName = "TalkinPoku/Apariencia de mascota", fileName = "Apariencia_")]
public class PetAppearance : ScriptableObject
{
    [Header("Expresiones")]
    [SerializeField] private Sprite normal;
    [SerializeField] private Sprite contento;
    [SerializeField] private Sprite conHambre;
    [SerializeField] private Sprite sucio;
    [SerializeField] private Sprite aburrido;
    [SerializeField] private Sprite cansado;
    [SerializeField] private Sprite dormido;
    [SerializeField] private Sprite acariciado;

    [Header("Minijuegos")]
    [SerializeField] private Sprite avion;

    [Header("Accesorios")]
    // Donde va cada ranura, como fraccion del sprite (0,0 abajo a la izquierda, 1,1 arriba a la
    // derecha), medido sobre la cara normal. Las otras caras tienen la cabeza en el mismo lugar.
    [SerializeField] private Vector2 anclaCabeza = new Vector2(0.45f, 0.7f);
    [SerializeField] private Vector2 anclaCara = new Vector2(0.45f, 0.5f);
    [SerializeField] private Vector2 anclaCuello = new Vector2(0.45f, 0.4f);
    [SerializeField] private Vector2 anclaEspalda = new Vector2(0.45f, 0.35f);
    // Las evoluciones tienen la cabeza mas chica en proporcion al cuerpo.
    [SerializeField] private float escalaAccesorios = 1f;

    [Header("Provisorio")]
    // Mientras no esten los dibujos de una evolucion se usan los de Poku mas grandes y tenidos, asi el
    // cambio se nota. Con el arte final, escala 1 y tinte blanco.
    [SerializeField] private float escala = 1f;
    [SerializeField] private Color tinte = Color.white;

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
    public Color Tinte => tinte;
}
