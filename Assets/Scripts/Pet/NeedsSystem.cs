using System;
using System.Globalization;
using UnityEngine;

// ============================================================================
// NeedsSystem.cs
// QUE HACE: el corazon de la mascota. Guarda las necesidades de Poku (hambre,
//           higiene, diversion y energia, de 0 a 100), las hace bajar con el
//           tiempo, las sube cuando se lo cuida y calcula la felicidad.
//           Tambien maneja si esta dormido.
// DONDE ESTA: en el objeto "Sistemas" de SampleScene.
// PARA CAMBIAR: casi todos los numeros (cuanto baja por hora, valor inicial,
//               umbrales, recuperacion durmiendo, bonus de la Mega) estan en el
//               asset Data/NeedsConfig.asset, NO aca.
//               Que tan rapido pasa el tiempo para probar -> multiplicadorTiempo
//               (Inspector de Sistemas, o el panel de admin).
// USO DESDE OTRO SCRIPT: necesidades.Sumar(NeedType.Hambre, 20);
//                        necesidades.Obtener(NeedType.Energia);
//                        necesidades.Dormir(true);
// AVISA: NeedChanged(tipo, valor) cuando cambia un valor (lo escuchan las barras
//        y la cara) y DormidoCambiado(bool) al dormirse o despertarse.
// ============================================================================

// Guarda el estado de las necesidades de la mascota y avisa cuando cambian. No conoce la UI ni
// las habitaciones: las barras y las actividades se enganchan a los eventos.
public class NeedsSystem : MonoBehaviour
{
    // Tope de cada necesidad.
    public const float Maximo = 100f;

    [Header("Referencias")]
    // El asset con todos los numeros (Data/NeedsConfig.asset).
    [SerializeField] private NeedsConfig config;

    [Header("Debug")]
    // [AJUSTE] Velocidad del tiempo para probar.
    // Para probar en el Editor sin esperar horas: con 3600 una hora de juego pasa en un segundo.
    [SerializeField] private float multiplicadorTiempo = 1f;

    public event Action<NeedType, float> NeedChanged;
    public event Action<bool> DormidoCambiado;

    // Hambre, Higiene, Diversion y Energia, en el orden del enum NeedType.
    private readonly float[] valores = new float[4];

    public bool Dormido { get; private set; }
    public float UmbralBajo => config.UmbralBajo;
    public float UmbralCritico => config.UmbralCritico;

    public float MultiplicadorTiempo
    {
        get => multiplicadorTiempo;
        set => multiplicadorTiempo = value;
    }

    // Felicidad = promedio de las otras cuatro.
    // Felicidad no se guarda: se calcula siempre, asi nunca queda desincronizada del resto.
    public float Felicidad =>
        (Obtener(NeedType.Hambre) + Obtener(NeedType.Higiene) + Obtener(NeedType.Diversion) + Obtener(NeedType.Energia)) / 4f;

    // Cuantas manchas de suciedad tiene Poku segun su higiene (0 a ManchasMaximas).
    public int Manchas
    {
        get
        {
            float falta = config.HigieneSinManchas - valores[(int)NeedType.Higiene];
            return Mathf.Clamp(Mathf.CeilToInt(falta / TramoPorMancha), 0, config.ManchasMaximas);
        }
    }

    // Multiplicador de los cuidados: x2 (o lo que diga NeedsConfig) con la Mega activa, x1 sin ella.
    // Lo que rinde cada cuidado. Se mira en el momento y no se guarda: si la Mega termina, deja de valer.
    private float Bonus => MegaEvolution.Activa ? config.MultiplicadorMega : 1f;
    private float EnergiaDurmiendoPorHora => config.EnergiaDurmiendoPorHora * Bonus;

    // Cuanta higiene representa cada mancha.
    private float TramoPorMancha => config.HigieneSinManchas / config.ManchasMaximas;

    // Ir a un minijuego recarga la escena de la casa, y con ella este componente. Lo estatico
    // sobrevive a ese cambio de escena. Al abrir la app lo llena el guardado.
    private static NeedsState estadoAlSalir;
    // La de la casa mientras esta cargada, para que el guardado pueda sacarle una foto desde afuera.
    private static NeedsSystem enEscena;

    // Para el guardado.
    // Desde un minijuego no hay casa cargada: se guarda la foto que quedo al salir.
    public static NeedsState Foto()
    {
        return enEscena != null ? enEscena.SacarFoto() : estadoAlSalir;
    }

    // Al abrir el juego: recibe lo guardado.
    // Awake la toma igual que al volver de un minijuego y descuenta el tiempo que estuvo cerrada.
    public static void Restaurar(NeedsState guardado)
    {
        estadoAlSalir = guardado;
    }

    // Arranca con lo guardado (o con el valor inicial) y descuenta el tiempo que paso afuera.
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

    // Al salir de la casa (ir a un minijuego) deja una foto del estado.
    private void OnDestroy()
    {
        estadoAlSalir = SacarFoto();
        if (enEscena == this) enEscena = null;
    }

    // Copia del estado actual con la hora, para guardar.
    private NeedsState SacarFoto()
    {
        return new NeedsState
        {
            valores = (float[])valores.Clone(),
            dormido = Dormido,
            momentoUtc = DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)
        };
    }

    // Cada frame avanza el tiempo (deltaTime pasado a horas).
    private void Update()
    {
        Avanzar(Time.deltaTime * multiplicadorTiempo / 3600f);
    }

    // Hace pasar "horas" de tiempo: baja las necesidades y, si duerme, recupera energia.
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

            // Se lleno la energia: se despierta solo.
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

    // Lee el valor de una necesidad (0 a 100). Es lo que muestran las barras.
    public float Obtener(NeedType tipo)
    {
        if (tipo == NeedType.Felicidad) return Felicidad;

        // La higiene se muestra de a manchas y no de a poco: 100 menos un pedazo igual por cada una.
        // Asi la barra esta llena mientras no haya nada para lavar.
        if (tipo == NeedType.Higiene) return Maximo - Manchas * (Maximo / config.ManchasMaximas);

        return valores[(int)tipo];
    }

    // La llama PetStains cuando la esponja borra una mancha.
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

    // Sube (o baja, con negativo) una necesidad. Es lo que llaman la comida, la pelota, las caricias, etc.
    // Lo que sube se multiplica por el Bonus de la Mega.
    public void Sumar(NeedType tipo, float cantidad)
    {
        // Felicidad no se sube directo: sube cuando sube cualquiera de las otras.
        if (tipo == NeedType.Felicidad) return;
        // Solo lo que sube: el panel de admin tambien resta por aca.
        Cambiar(tipo, cantidad > 0f ? cantidad * Bonus : cantidad);
    }

    // Lo acuesta (true) o lo despierta (false) y avisa.
    public void Dormir(bool dormir)
    {
        if (Dormido == dormir) return;
        Dormido = dormir;
        DormidoCambiado?.Invoke(Dormido);
    }

    // Unico lugar donde se modifica un valor: lo limita entre 0 y 100 y avisa si cambio lo que se ve.
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
