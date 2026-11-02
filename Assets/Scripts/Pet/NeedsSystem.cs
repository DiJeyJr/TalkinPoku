using System;
using System.Globalization;
using UnityEngine;

// Guarda el estado de las necesidades de la mascota y avisa cuando cambian. No conoce la UI ni
// las habitaciones: las barras y las actividades se enganchan a los eventos.
public class NeedsSystem : MonoBehaviour
{
    public const float Maximo = 100f;

    [Header("Referencias")]
    [SerializeField] private NeedsConfig config;

    [Header("Debug")]
    // Para probar en el Editor sin esperar horas: con 3600 una hora de juego pasa en un segundo.
    [SerializeField] private float multiplicadorTiempo = 1f;

    public event Action<NeedType, float> NeedChanged;
    public event Action<bool> DormidoCambiado;

    private readonly float[] valores = new float[4];

    public bool Dormido { get; private set; }
    public float UmbralBajo => config.UmbralBajo;
    public float UmbralCritico => config.UmbralCritico;

    public float MultiplicadorTiempo
    {
        get => multiplicadorTiempo;
        set => multiplicadorTiempo = value;
    }

    // Felicidad no se guarda: se calcula siempre, asi nunca queda desincronizada del resto.
    public float Felicidad =>
        (Obtener(NeedType.Hambre) + Obtener(NeedType.Higiene) + Obtener(NeedType.Diversion) + Obtener(NeedType.Energia)) / 4f;

    public int Manchas
    {
        get
        {
            float falta = config.HigieneSinManchas - valores[(int)NeedType.Higiene];
            return Mathf.Clamp(Mathf.CeilToInt(falta / TramoPorMancha), 0, config.ManchasMaximas);
        }
    }

    // Lo que rinde cada cuidado. Se mira en el momento y no se guarda: si la Mega termina, deja de valer.
    private float Bonus => MegaEvolution.Activa ? config.MultiplicadorMega : 1f;
    private float EnergiaDurmiendoPorHora => config.EnergiaDurmiendoPorHora * Bonus;

    private float TramoPorMancha => config.HigieneSinManchas / config.ManchasMaximas;

    // Ir a un minijuego recarga la escena de la casa, y con ella este componente. Lo estatico
    // sobrevive a ese cambio de escena. Al abrir la app lo llena el guardado.
    private static NeedsState estadoAlSalir;
    // La de la casa mientras esta cargada, para que el guardado pueda sacarle una foto desde afuera.
    private static NeedsSystem enEscena;

    // Desde un minijuego no hay casa cargada: se guarda la foto que quedo al salir.
    public static NeedsState Foto()
    {
        return enEscena != null ? enEscena.SacarFoto() : estadoAlSalir;
    }

    // Awake la toma igual que al volver de un minijuego y descuenta el tiempo que estuvo cerrada.
    public static void Restaurar(NeedsState guardado)
    {
        estadoAlSalir = guardado;
    }

    private void Awake()
    {
        enEscena = this;

        // Sin foto (primera vez que se abre) o con una que no cierra (guardado roto): valores iniciales.
        if (estadoAlSalir == null || estadoAlSalir.valores == null || estadoAlSalir.valores.Length != valores.Length)
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

        // La mascota siguio viviendo mientras se jugaba o con la app cerrada: se descuenta ese tiempo de
        // una sola vez. Con tope, para no encontrarla en cero despues de unos dias, y en cero si el reloj
        // del celular va para atras, para no regalar necesidades.
        float horas = 0f;
        if (DateTime.TryParse(estadoAlSalir.momentoUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime salida))
        {
            horas = (float)(DateTime.UtcNow - salida).TotalHours * multiplicadorTiempo;
        }
        Avanzar(Mathf.Clamp(horas, 0f, config.HorasMaximasAusente));
    }

    private void OnDestroy()
    {
        estadoAlSalir = SacarFoto();
        if (enEscena == this) enEscena = null;
    }

    private NeedsState SacarFoto()
    {
        return new NeedsState
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

    // Recibe horas y no segundos porque al abrir la app se usa este mismo metodo con el tiempo que
    // estuvo cerrada (DateTime.UtcNow contra el momento en que se guardo).
    private void Avanzar(float horas)
    {
        if (Dormido)
        {
            // Con la app cerrada pueden ser horas: si la energia se llena antes de que termine el tramo,
            // se despierta en ese momento y lo que sobra corre despierto.
            float hastaLlenarse = (Maximo - valores[(int)NeedType.Energia]) / EnergiaDurmiendoPorHora;
            float dormidas = Mathf.Min(horas, hastaLlenarse);

            Bajar(dormidas * config.FactorDormido);
            Cambiar(NeedType.Energia, EnergiaDurmiendoPorHora * dormidas);
            if (horas < hastaLlenarse) return;

            Dormir(false);
            horas -= dormidas;
        }

        Bajar(horas);
        Cambiar(NeedType.Energia, -config.DecaimientoPorHora(NeedType.Energia) * horas);
    }

    // Hambre, higiene y diversion bajan igual despierto o dormido; durmiendo solo cuentan menos horas.
    private void Bajar(float horas)
    {
        Cambiar(NeedType.Hambre, -config.DecaimientoPorHora(NeedType.Hambre) * horas);
        Cambiar(NeedType.Higiene, -config.DecaimientoPorHora(NeedType.Higiene) * horas);
        Cambiar(NeedType.Diversion, -config.DecaimientoPorHora(NeedType.Diversion) * horas);
    }

    public float Obtener(NeedType tipo)
    {
        if (tipo == NeedType.Felicidad) return Felicidad;

        // La higiene se muestra de a manchas y no de a poco: 100 menos un pedazo igual por cada una.
        // Asi la barra esta llena mientras no haya nada para lavar.
        if (tipo == NeedType.Higiene) return Maximo - Manchas * (Maximo / config.ManchasMaximas);

        return valores[(int)tipo];
    }

    // La ultima mancha deja la higiene en 100: banarlo lo deja limpio del todo.
    public void LimpiarMancha()
    {
        int manchas = Manchas;
        if (manchas == 0) return;

        int i = (int)NeedType.Higiene;
        float nuevo = manchas == 1 ? Maximo : valores[i] + TramoPorMancha;
        Cambiar(NeedType.Higiene, nuevo - valores[i]);
    }

    // Para el panel de admin: ensucia lo justo para que aparezca una mancha mas.
    public void AgregarMancha()
    {
        int manchas = Manchas;
        if (manchas >= config.ManchasMaximas) return;

        int i = (int)NeedType.Higiene;
        // Al medio del tramo siguiente, asi el decaimiento de los proximos frames no la cambia de tramo.
        float nuevo = config.HigieneSinManchas - (manchas + 0.5f) * TramoPorMancha;
        Cambiar(NeedType.Higiene, Mathf.Min(nuevo, valores[i]) - valores[i]);
    }

    public void Sumar(NeedType tipo, float cantidad)
    {
        // Felicidad no se sube directo: sube cuando sube cualquiera de las otras.
        if (tipo == NeedType.Felicidad) return;
        // Solo lo que sube: el panel de admin tambien resta por aca.
        Cambiar(tipo, cantidad > 0f ? cantidad * Bonus : cantidad);
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
        float antes = Obtener(tipo);
        float felicidadAntes = Felicidad;
        valores[i] = Mathf.Clamp(valores[i] + delta, 0f, Maximo);

        // El decaimiento mueve los valores un poquito en cada frame, y cada aviso repinta las barras
        // y la cara. Se avisa solo cuando cambia lo que se ve: el numero redondeado, o una mancha en
        // la higiene.
        float despues = Obtener(tipo);
        if (Mathf.Round(despues) != Mathf.Round(antes)) NeedChanged?.Invoke(tipo, despues);
        if (Mathf.Round(Felicidad) != Mathf.Round(felicidadAntes)) NeedChanged?.Invoke(NeedType.Felicidad, Felicidad);
    }
}
