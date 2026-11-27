using TMPro;
using UnityEngine;

// ============================================================================
// VueloGame.cs  (minijuego "Vuelo")
// QUE HACE: las reglas del vuelo: un punto por cada obstaculo que se pasa; la
//           partida termina al chocar. Se gana si se llego a puntosVictoria.
// DONDE ESTA: en el objeto "Minijuego" de la escena Vuelo.
// PARA CAMBIAR (Inspector de "Minijuego"):
//   - monedas por punto / tope de monedas -> monedasPorPunto / maximoMonedas
//   - puntos para ganar -> puntosVictoria
//   - velocidad y frecuencia de los obstaculos -> ObstaculoSpawner ("Obstaculos")
//   - fuerza del aleteo -> Volador ("Volador")
// EJEMPLO DE CAMBIO TIPICO: "dar vidas extra" -> agregar un int vidas, y en
//   Perder() restar una y solo terminar si vidas llega a 0 (ver AtraparGame,
//   que ya tiene vidas).
// ============================================================================

// Reglas del minijuego de vuelo: un punto por obstaculo que se pasa, y se termina al chocar.
public class VueloGame : Minijuego
{
    [Header("Referencias")]
    [SerializeField] private Volador volador;
    [SerializeField] private ObstaculoSpawner spawner;
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private GameObject indicacionInicio;

    [Header("Recompensa")]
    // [AJUSTE] Monedas por punto y maximo por partida.
    [SerializeField] private int monedasPorPunto = 1;
    [SerializeField] private int maximoMonedas = 50;

    [Header("Victoria")]
    // [AJUSTE] Puntos para que cuente como victoria.
    // El vuelo es infinito y siempre termina chocando: se gana si se llego a esta cantidad antes.
    [SerializeField] private int puntosVictoria = 10;
    // Pausa entre el choque y el resultado, para que se vea caer a la mascota antes de tapar todo.
    [SerializeField] private float segundosAntesDelResultado = 0.8f;

    [Header("Sonidos")]
    // Cada obstaculo pasado es una moneda ganada.
    [SerializeField] private AudioClip sonidoPunto;

    private int puntos;

    // Arranca: puntos en 0, muestra la indicacion y escucha al volador.
    public override void StartGame()
    {
        puntos = 0;
        textoPuntos.SetText("{0}", puntos);
        indicacionInicio.SetActive(true);

        volador.Despego += AlDespegar;
        volador.PasoObstaculo += SumarPunto;
        volador.Choco += Perder;
    }

    // Primer tap: esconde la indicacion y empiezan los obstaculos.
    private void AlDespegar()
    {
        indicacionInicio.SetActive(false);
        spawner.Arrancar();
    }

    // Paso un obstaculo: +1 punto y sube la dificultad.
    private void SumarPunto()
    {
        puntos++;
        textoPuntos.SetText("{0}", puntos);
        SoundPlayer.Reproducir(sonidoPunto);
        // La dificultad depende del puntaje, no del tiempo: quien juega mal no se ahoga mas rapido.
        spawner.ActualizarDificultad(puntos);
    }

    // Choco: frena todo, calcula monedas y termina.
    private void Perder()
    {
        spawner.Frenar();

        // Lineal con techo, para que una partida muy larga no rompa la economia.
        int monedas = Mathf.Min(puntos * monedasPorPunto, maximoMonedas);
        Terminar(new MinigameResult(puntos, monedas, true, puntos >= puntosVictoria), segundosAntesDelResultado);
    }
}
