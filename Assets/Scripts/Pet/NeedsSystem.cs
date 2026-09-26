using System;
using System.Globalization;
using UnityEngine;

/// Guarda el estado de las necesidades de la mascota y avisa cuando cambian. No conoce la UI ni
/// las habitaciones: las barras y las actividades se enganchan a los eventos.
public class NeedsSystem : MonoBehaviour
{
    private const float Maximo = 100f;

    [Header("Referencias")]
    [SerializeField] private NeedsConfig config;

    [Header("Debug")]
    // Para probar en el Editor sin esperar horas: con 3600 una hora de juego pasa en un segundo.
    [SerializeField] private float multiplicadorTiempo = 1f;

    public event Action<NeedType, float> NeedChanged;
    public event Action<NeedType> NeedCritical;
    public event Action<bool> DormidoCambiado;

    // Hambre, Higiene, Diversion y Energia, en el orden del enum.
    private readonly float[] valores = new float[4];

    public bool Dormido { get; private set; }
    public float UmbralCritico => config.UmbralCritico;

    // Felicidad no se guarda: se calcula siempre, asi nunca queda desincronizada del resto.
    public float Felicidad => (valores[0] + valores[1] + valores[2] + valores[3]) / 4f;

    // Ir a un minijuego recarga la escena de la casa, y con ella este componente. Lo estatico
    // sobrevive a ese cambio de escena; se pierde al cerrar la app, que es lo que va a cubrir el guardado.
    private static NeedsState estadoAlSalir;

    private void Awake()
    {
        if (estadoAlSalir == null)
        {
            for (int i = 0; i < valores.Length; i++)
            {
                valores[i] = config.ValorInicial;
            }
            return;
        }

        Array.Copy(estadoAlSalir.valores, valores, valores.Length);
        // Con Dormir y no asignando Dormido directo, para que el overlay se entere si ya estaba escuchando.
        Dormir(estadoAlSalir.dormido);

        // La mascota siguio viviendo mientras se jugaba: se descuenta ese tiempo de una sola vez.
        DateTime salida = DateTime.Parse(estadoAlSalir.momentoUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        Avanzar((float)(DateTime.UtcNow - salida).TotalHours * multiplicadorTiempo);
    }

    private void OnDestroy()
    {
        estadoAlSalir = new NeedsState
        {
            valores = (float[])valores.Clone(),
            dormido = Dormido,
            momentoUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
        };
    }

    private void Update()
    {
        Avanzar(Time.deltaTime * multiplicadorTiempo / 3600f);
    }

    // Recibe horas y no segundos porque el guardado va a usar este mismo metodo con el tiempo que
    // estuvo cerrada la app (DateTime.UtcNow contra el momento en que se guardo).
    public void Avanzar(float horas)
    {
        Cambiar(NeedType.Hambre, -config.DecaimientoPorHora(NeedType.Hambre) * horas);
        Cambiar(NeedType.Higiene, -config.DecaimientoPorHora(NeedType.Higiene) * horas);
        Cambiar(NeedType.Diversion, -config.DecaimientoPorHora(NeedType.Diversion) * horas);

        if (Dormido)
        {
            Cambiar(NeedType.Energia, config.EnergiaDurmiendoPorHora * horas);
            if (valores[(int)NeedType.Energia] >= Maximo) Dormir(false);
        }
        else
        {
            Cambiar(NeedType.Energia, -config.DecaimientoPorHora(NeedType.Energia) * horas);
        }
    }

    public float Obtener(NeedType tipo)
    {
        if (tipo == NeedType.Felicidad) return Felicidad;
        return valores[(int)tipo];
    }

    public void Sumar(NeedType tipo, float cantidad)
    {
        // Felicidad no se sube directo: sube cuando sube cualquiera de las otras.
        if (tipo == NeedType.Felicidad) return;
        Cambiar(tipo, cantidad);
    }

    public void Dormir(bool dormir)
    {
        if (Dormido == dormir) return;
        Dormido = dormir;
        DormidoCambiado?.Invoke(Dormido);
    }

    private void Cambiar(NeedType tipo, float delta)
    {
        int i = (int)tipo;
        float antes = valores[i];
        float despues = Mathf.Clamp(antes + delta, 0f, Maximo);
        if (despues == antes) return;

        valores[i] = despues;
        NeedChanged?.Invoke(tipo, despues);
        NeedChanged?.Invoke(NeedType.Felicidad, Felicidad);

        // Solo al cruzar el umbral, no en cada frame que sigue abajo.
        if (antes >= config.UmbralCritico && despues < config.UmbralCritico)
        {
            NeedCritical?.Invoke(tipo);
        }
    }
}
