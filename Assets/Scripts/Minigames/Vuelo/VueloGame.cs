using TMPro;
using UnityEngine;

/// Reglas del minijuego de vuelo: un punto por obstaculo que se pasa, y se termina al chocar.
public class VueloGame : Minijuego
{
    [Header("Referencias")]
    [SerializeField] private Volador volador;
    [SerializeField] private ObstaculoSpawner spawner;
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private GameObject indicacionInicio;

    [Header("Recompensa")]
    [SerializeField] private int monedasPorPunto = 1;
    [SerializeField] private int maximoMonedas = 50;

    [Header("Victoria")]
    // El vuelo es infinito y siempre termina chocando: se gana si se llego a esta cantidad antes.
    [SerializeField] private int puntosVictoria = 10;
    // Pausa entre el choque y el resultado, para que se vea caer a la mascota antes de tapar todo.
    [SerializeField] private float segundosAntesDelResultado = 0.8f;

    [Header("Sonidos")]
    // Cada obstaculo pasado es una moneda ganada.
    [SerializeField] private AudioClip sonidoPunto;

    private int puntos;

    public override void StartGame()
    {
        puntos = 0;
        textoPuntos.SetText("{0}", puntos);
        indicacionInicio.SetActive(true);

        volador.Despego += AlDespegar;
        volador.PasoObstaculo += SumarPunto;
        volador.Choco += Perder;
    }

    private void AlDespegar()
    {
        indicacionInicio.SetActive(false);
        spawner.Arrancar();
    }

    private void SumarPunto()
    {
        puntos++;
        textoPuntos.SetText("{0}", puntos);
        SoundPlayer.Reproducir(sonidoPunto);
        // La dificultad depende del puntaje, no del tiempo: quien juega mal no se ahoga mas rapido.
        spawner.ActualizarDificultad(puntos);
    }

    private void Perder()
    {
        spawner.Frenar();

        // Lineal con techo, para que una partida muy larga no rompa la economia.
        int monedas = Mathf.Min(puntos * monedasPorPunto, maximoMonedas);
        Terminar(new MinigameResult(puntos, monedas, true, puntos >= puntosVictoria), segundosAntesDelResultado);
    }
}
