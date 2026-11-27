using System;
using UnityEngine;

// ============================================================================
// MegaEvolution.cs
// QUE HACE: la Mega Evolucion. Desde el nivel maximo, la XP que gana el jugador
//           carga una barra de "Energia Mega". Con la barra llena se puede
//           activar: Pokuchan pasa a Mega-Pokuchan por un tiempo y los
//           minijuegos y cuidados rinden mas.
// DONDE ESTA: en el objeto "Sistemas" de SampleScene. El boton es MegaButton.
// PARA CAMBIAR (en Data/Progression/ProgressionConfig.asset, seccion Mega):
//   - desde que nivel se carga -> Nivel Mega
//   - cuanta energia hace falta -> Energia Mega
//   - cuanto dura -> Minutos Mega
//   - bonus de monedas -> Multiplicador Monedas Mega
//   - bonus en comida/juegos/sueno -> Multiplicador Mega en NeedsConfig.asset
// USO DESDE OTRO SCRIPT: MegaEvolution.Activa (true si esta activa ahora).
// AVISA: EnergiaCambiada(fraccion 0..1), MegaCambiada(activa) y PrimeraVez.
// ============================================================================

// La Mega Evolucion: desde el nivel 50 las actividades cargan la Energia Mega, y con la barra
// llena el jugador puede convertir a Pokuchan en Mega-Pokuchan por media hora. Mientras dura los
// minijuegos pagan mas monedas (MinigameReward).
public class MegaEvolution : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Progression progreso;

    // Cuanto falta para poder activarla, o cuanto le queda si esta activa (de 0 a 1).
    public event Action<float> EnergiaCambiada;
    public event Action<bool> MegaCambiada;
    // Solo la primera vez que se activa en la partida: el cartel que explica que gana con la Mega.
    public event Action PrimeraVez;

    // Estatico como el nivel: sobrevive a ir a un minijuego y lo llena el guardado.
    private static int energia;
    // Hasta cuando dura, en hora real (ticks UTC) y no en Time.time: la media hora sigue corriendo
    // con la app cerrada, como las necesidades. 0 = no esta activa.
    private static long megaHasta;
    // Si ya vio el cartel de la primera Mega. Se guarda: no tiene que volver a salir al reabrir la app.
    private static bool presentada;

    private bool estabaActiva;
    private int segundosAvisados;

    public static int EnergiaActual => energia;
    public static long MegaHasta => megaHasta;
    public static bool Presentada => presentada;
    // Se calcula con la hora y no con un flag: los minijuegos lo leen sin tener este componente.
    public static bool Activa => megaHasta > DateTime.UtcNow.Ticks;

    // Al abrir el juego: recupera la energia, hasta cuando dura y si ya vio el cartel.
    public static void Restaurar(int energiaGuardada, long megaHastaGuardada, bool presentadaGuardada)
    {
        energia = Mathf.Max(0, energiaGuardada);
        megaHasta = megaHastaGuardada;
        presentada = presentadaGuardada;
    }

    private ProgressionConfig Config => progreso.Config;
    // true si ya llego al nivel de la Mega.
    public bool Desbloqueada => progreso.Nivel >= Config.NivelMega;
    // true si se puede activar ahora (desbloqueada, no activa y con la barra llena).
    public bool Lista => Desbloqueada && !Activa && energia >= Config.EnergiaMega;

    // Lo que muestra la barra del boton: tiempo restante si esta activa, carga si no.
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

    // Mientras esta activa: revisa si se termino el tiempo y avisa una vez por segundo.
    private void Update()
    {
        if (!estabaActiva) return;

        // Ningun evento avisa cuando pasa la media hora: se mira el reloj.
        if (!Activa)
        {
            Terminar();
            return;
        }

        // El boton muestra minutos y segundos: con avisar una vez por segundo alcanza.
        int segundos = Mathf.CeilToInt((float)SegundosRestantes);
        if (segundos == segundosAvisados) return;
        segundosAvisados = segundos;
        EnergiaCambiada?.Invoke(Fraccion);
    }

    // La llama el boton Mega. Vacia la barra y arranca el tiempo.
    public void Activar()
    {
        if (!Lista) return;

        energia = 0;
        megaHasta = DateTime.UtcNow.AddMinutes(Config.MinutosMega).Ticks;
        estabaActiva = true;
        MegaCambiada?.Invoke(true);
        EnergiaCambiada?.Invoke(Fraccion);
        if (!presentada)
        {
            presentada = true;
            PrimeraVez?.Invoke();
        }

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

    // Cada vez que el jugador gana XP, la misma cantidad carga la barra de la Mega.
    private void AlPremiar(int xp)
    {
        // Mientras dura no se carga: si no, al terminar ya estaria lista para otra media hora.
        if (!Desbloqueada || Activa || energia >= Config.EnergiaMega) return;
        energia = Mathf.Min(Config.EnergiaMega, energia + xp);
        EnergiaCambiada?.Invoke(Fraccion);
    }
}
