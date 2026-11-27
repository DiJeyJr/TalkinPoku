using UnityEngine;

// ============================================================================
// PetStains.cs
// QUE HACE: maneja las manchas de suciedad sobre Poku. NeedsSystem dice cuantas
//           tiene que haber (segun la higiene) y este script prende esas. Cuando
//           la esponja borra una, le avisa a NeedsSystem para subir la higiene.
// DONDE ESTA: en "Poku/Manchas" de SampleScene. Cada mancha es un hijo con Stain.
// PARA CAMBIAR: el orden en que aparecen -> lista "Manchas" del Inspector.
//               cuantas manchas como maximo / desde que higiene -> NeedsConfig.asset.
//               XP por limpiar -> ProgressionConfig.asset (fuente "Limpiar").
// ============================================================================

// Las manchas de Poku. Cuantas hay lo decide NeedsSystem (la higiene se mide en manchas); este
// script prende las que tocan y, cuando la esponja borra una, le avisa para que la descuente.
public class PetStains : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    // [AJUSTE] Las manchas posibles.
    // En el orden en que aparecen: la primera es la que se ve apenas empieza a ensuciarse.
    [SerializeField] private Stain[] manchas;
    [SerializeField] private Progression progreso;

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

    // La llama la esponja (Sponge) mientras se mueve.
    // Frota todas las que la esponja tiene abajo, no solo una: si cubre dos, limpia las dos. Devuelve
    // si toco alguna, mirado antes de frotar: la pasada que borra la ultima tambien hace espuma.
    public bool Frotar(Vector2 centro, float alcance, float distancia)
    {
        bool tocoAlguna = false;
        foreach (Stain mancha in manchas)
        {
            if (!mancha.Activa || !mancha.Toca(centro, alcance)) continue;
            tocoAlguna = true;
            mancha.Frotar(distancia);
        }
        return tocoAlguna;
    }

    private void AlCambiar(NeedType tipo, float valor)
    {
        // La higiene solo avisa cuando cambia la cantidad de manchas, no en cada frame.
        if (tipo == NeedType.Higiene) Sincronizar();
    }

    // Se borro una mancha: sube la higiene, suena y da XP.
    private void AlLimpiar()
    {
        // La mancha ya se apago antes de avisar: al descontarla, la cuenta de NeedsSystem da una
        // menos y Sincronizar no apaga ninguna otra.
        necesidades.LimpiarMancha();
        SoundPlayer.Reproducir(sonidoLimpiar);
        if (progreso != null) progreso.Ganar(XpSource.Limpiar);
    }

    // Prende o apaga manchas hasta que queden tantas como dice NeedsSystem.
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
