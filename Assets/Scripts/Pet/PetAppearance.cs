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
    public Color Tinte => tinte;
}
