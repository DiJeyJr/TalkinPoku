using System;
using UnityEngine;

/// Nivel y experiencia de la mascota. Las actividades le avisan con Ganar y este decide cuanto vale
/// cada una (ProgressionConfig). Avisa por evento cuando cambia la experiencia, cuando sube de nivel
/// y cuando eso la hace evolucionar.
public class Progression : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ProgressionConfig config;
    // Para premiar una dormida completa: no hay un script de "dormir" que pueda avisar al terminar.
    [SerializeField] private NeedsSystem necesidades;

    // Experiencia actual y la que pide el nivel actual (0 en el nivel maximo).
    public event Action<int, int> XpCambiada;
    public event Action<int> NivelSubido;
    public event Action<int> EtapaCambiada;
    // Cada premio que paso la pausa, aunque ya este en el nivel maximo: con eso se carga la Mega.
    public event Action<int> Premiado;

    // Estatico como el saldo de Wallet: sobrevive a ir a un minijuego y volver. Al abrir la app lo
    // llena el guardado.
    private static int nivel = 1;
    private static int xp;

    // Cuando se puede volver a premiar cada fuente. No se guarda: al reabrir la app arranca de cero.
    private readonly float[] proximoPremio = new float[Enum.GetValues(typeof(XpSource)).Length];

    public static int NivelActual => nivel;
    public static int XpActual => xp;

    public static void Restaurar(int nivelGuardado, int xpGuardada)
    {
        // Una partida de antes de la progresion no trae nivel (queda en 0): arranca en el 1.
        if (nivelGuardado < 1) return;
        nivel = nivelGuardado;
        xp = Mathf.Max(0, xpGuardada);
    }

    public ProgressionConfig Config => config;
    public int Nivel => nivel;
    public int Xp => xp;
    public int XpRequerida => nivel >= config.NivelMaximo ? 0 : config.XpParaSubir(nivel);
    public int Etapa => config.EtapaDe(nivel);

    private void OnEnable()
    {
        if (necesidades != null) necesidades.DormidoCambiado += AlCambiarDormido;
    }

    private void OnDisable()
    {
        if (necesidades != null) necesidades.DormidoCambiado -= AlCambiarDormido;
    }

    public void Ganar(XpSource fuente)
    {
        if (!config.BuscarPremio(fuente, out ProgressionConfig.Premio premio)) return;

        int i = (int)fuente;
        if (Time.time < proximoPremio[i]) return;
        proximoPremio[i] = Time.time + premio.segundosEntrePremios;

        Sumar(premio.xp);
        Premiado?.Invoke(premio.xp);
    }

    // Publico para el panel de admin; el juego suma siempre con Ganar.
    public void Sumar(int cantidad)
    {
        if (cantidad <= 0 || nivel >= config.NivelMaximo) return;

        int etapaAntes = Etapa;
        xp += cantidad;

        // Un premio grande puede alcanzar para mas de un nivel: se suben todos los que alcance.
        while (nivel < config.NivelMaximo && xp >= config.XpParaSubir(nivel))
        {
            xp -= config.XpParaSubir(nivel);
            nivel++;
            NivelSubido?.Invoke(nivel);
        }
        if (nivel >= config.NivelMaximo) xp = 0;

        XpCambiada?.Invoke(xp, XpRequerida);
        if (Etapa != etapaAntes) EtapaCambiada?.Invoke(Etapa);
    }

    // Para el panel de admin.
    public void SubirNivel()
    {
        Sumar(XpRequerida - xp);
    }

    // Para el panel de admin: vuelve a Poku en el nivel 1.
    public void Reiniciar()
    {
        int etapaAntes = Etapa;
        nivel = 1;
        xp = 0;
        XpCambiada?.Invoke(xp, XpRequerida);
        if (Etapa != etapaAntes) EtapaCambiada?.Invoke(Etapa);
    }

    private void AlCambiarDormido(bool dormido)
    {
        // Solo una dormida completa: NeedsSystem lo despierta solo al llenar la energia. Despertarlo
        // antes con la cama no premia, asi acostarlo y levantarlo no es una forma de subir.
        if (!dormido && necesidades.Obtener(NeedType.Energia) >= NeedsSystem.Maximo - 0.5f) Ganar(XpSource.Dormir);
    }
}
