using UnityEngine;

/// Las manchas de Poku. Cuantas hay lo decide NeedsSystem (la higiene se mide en manchas); este
/// script prende las que tocan y, cuando la esponja borra una, le avisa para que la descuente.
public class PetStains : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    // En el orden en que aparecen: la primera es la que se ve apenas empieza a ensuciarse.
    [SerializeField] private Stain[] manchas;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoLimpiar;

    private void OnEnable()
    {
        necesidades.NeedChanged += AlCambiar;
        foreach (Stain mancha in manchas) mancha.Limpiada += AlLimpiar;
    }

    private void OnDisable()
    {
        necesidades.NeedChanged -= AlCambiar;
        foreach (Stain mancha in manchas) mancha.Limpiada -= AlLimpiar;
    }

    private void Start()
    {
        Sincronizar();
    }

    // Frota todas las que la esponja tiene abajo, no solo una: si cubre dos, limpia las dos.
    public void Frotar(Vector2 centro, float alcance, float distancia)
    {
        foreach (Stain mancha in manchas)
        {
            if (mancha.Activa && mancha.Toca(centro, alcance)) mancha.Frotar(distancia);
        }
    }

    private void AlCambiar(NeedType tipo, float valor)
    {
        // La higiene solo avisa cuando cambia la cantidad de manchas, no en cada frame.
        if (tipo == NeedType.Higiene) Sincronizar();
    }

    private void AlLimpiar()
    {
        // La mancha ya se apago antes de avisar: al descontarla, la cuenta de NeedsSystem da una
        // menos y Sincronizar no apaga ninguna otra.
        necesidades.LimpiarMancha();
        SoundPlayer.Reproducir(sonidoLimpiar);
    }

    private void Sincronizar()
    {
        int objetivo = Mathf.Min(necesidades.Manchas, manchas.Length);

        int activas = 0;
        foreach (Stain mancha in manchas)
        {
            if (mancha.Activa) activas++;
        }

        // Se prenden en orden y se apagan desde el final, asi siempre se ensucia primero lo mismo.
        for (int i = 0; i < manchas.Length && activas < objetivo; i++)
        {
            if (manchas[i].Activa) continue;
            manchas[i].Ensuciar();
            activas++;
        }

        for (int i = manchas.Length - 1; i >= 0 && activas > objetivo; i--)
        {
            if (!manchas[i].Activa) continue;
            manchas[i].Quitar();
            activas--;
        }
    }
}
