using System;
using UnityEngine;

/// La Mega Evolucion: desde el nivel 50 las actividades cargan la Energia Mega, y con la barra
/// llena el jugador puede convertir a Pokuchan en Mega-Pokuchan por media hora. Mientras dura los
/// minijuegos pagan mas monedas (MinigameReward).
public class MegaEvolution : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Progression progreso;

    // Cuanto falta para poder activarla, o cuanto le queda si esta activa (de 0 a 1).
    public event Action<float> EnergiaCambiada;
    public event Action<bool> MegaCambiada;

    // Estatico como el nivel: sobrevive a ir a un minijuego y lo llena el guardado.
    private static int energia;
    // Hasta cuando dura, en hora real (ticks UTC) y no en Time.time: la media hora sigue corriendo
    // con la app cerrada, como las necesidades. 0 = no esta activa.
    private static long megaHasta;

    private bool estabaActiva;

    public static int EnergiaActual => energia;
    public static long MegaHasta => megaHasta;
    // Se calcula con la hora y no con un flag: los minijuegos lo leen sin tener este componente.
    public static bool Activa => megaHasta > DateTime.UtcNow.Ticks;

    public static void Restaurar(int energiaGuardada, long megaHastaGuardada)
    {
        energia = Mathf.Max(0, energiaGuardada);
        megaHasta = megaHastaGuardada;
    }

    private ProgressionConfig Config => progreso.Config;
    public bool Desbloqueada => progreso.Nivel >= Config.NivelMega;
    public bool Lista => Desbloqueada && !Activa && energia >= Config.EnergiaMega;

    public float Fraccion
    {
        get
        {
            if (Activa) return Mathf.Clamp01((float)(SegundosRestantes / (Config.MinutosMega * 60f)));
            return Mathf.Clamp01((float)energia / Config.EnergiaMega);
        }
    }

    public double SegundosRestantes => Activa ? TimeSpan.FromTicks(megaHasta - DateTime.UtcNow.Ticks).TotalSeconds : 0;

    private void OnEnable()
    {
        progreso.Premiado += AlPremiar;
    }

    private void OnDisable()
    {
        progreso.Premiado -= AlPremiar;
    }

    private void Start()
    {
        // Si se termino con la app cerrada, se arranca como Pokuchan sin avisar.
        estabaActiva = Activa;
    }

    private void Update()
    {
        if (!estabaActiva) return;

        // Ningun evento avisa cuando pasa la media hora: se mira el reloj.
        if (Activa) EnergiaCambiada?.Invoke(Fraccion);
        else Terminar();
    }

    public void Activar()
    {
        if (!Lista) return;

        energia = 0;
        megaHasta = DateTime.UtcNow.AddMinutes(Config.MinutosMega).Ticks;
        estabaActiva = true;
        MegaCambiada?.Invoke(true);
        EnergiaCambiada?.Invoke(Fraccion);

        // Si la app se cierra antes del proximo autoguardado, volveria a estar la barra llena.
        SaveSystem.Guardar();
    }

    // Para el panel de admin.
    public void LlenarEnergia()
    {
        if (Activa) return;
        energia = Config.EnergiaMega;
        EnergiaCambiada?.Invoke(Fraccion);
    }

    // Para el panel de admin: corta la Mega sin esperar la media hora.
    public void Terminar()
    {
        megaHasta = 0;
        if (!estabaActiva) return;
        estabaActiva = false;
        MegaCambiada?.Invoke(false);
        EnergiaCambiada?.Invoke(Fraccion);
    }

    private void AlPremiar(int xp)
    {
        // Mientras dura no se carga: si no, al terminar ya estaria lista para otra media hora.
        if (!Desbloqueada || Activa || energia >= Config.EnergiaMega) return;
        energia = Mathf.Min(Config.EnergiaMega, energia + xp);
        EnergiaCambiada?.Invoke(Fraccion);
    }
}
