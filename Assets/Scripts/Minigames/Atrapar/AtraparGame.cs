using System;
using System.Collections;
using TMPro;
using UnityEngine;

/// Reglas del minijuego de atrapar: un punto por ingrediente atrapado y tres vidas. Se pierde una
/// al atrapar algo malo o al dejar caer un ingrediente; sin vidas, termina la partida.
public class AtraparGame : MonoBehaviour, IMinigame
{
    [Header("Referencias")]
    [SerializeField] private Canasta canasta;
    [SerializeField] private CaidaSpawner spawner;
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private TMP_Text textoVidas;
    // Opcional: un corazon por vida, se apagan de derecha a izquierda.
    [SerializeField] private GameObject[] corazones;
    [SerializeField] private GameObject indicacionInicio;

    [Header("Vidas")]
    [SerializeField] private int vidasIniciales = 3;

    [Header("Recompensa")]
    [SerializeField] private int monedasPorPunto = 1;
    [SerializeField] private int maximoMonedas = 50;

    [Header("Victoria")]
    // Siempre termina sin vidas: se gana si antes se llego a este puntaje.
    [SerializeField] private int puntosVictoria = 25;
    // Tiempo con la indicacion en pantalla antes de que caiga lo primero, para acomodar el celular.
    [SerializeField] private float segundosAntesDeEmpezar = 1.5f;
    [SerializeField] private float segundosAntesDelResultado = 0.8f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoAtrapar;
    [SerializeField] private AudioClip sonidoMalo;
    [SerializeField] private AudioClip sonidoSeCayo;

    public event Action<MinigameResult> Finished;

    private int puntos;
    private int vidas;
    private bool terminado;

    public void StartGame()
    {
        puntos = 0;
        vidas = vidasIniciales;
        terminado = false;
        textoPuntos.SetText("{0}", puntos);
        MostrarVidas();

        canasta.Atrapo += AlAtrapar;
        spawner.SeCayoBueno += AlCaerseBueno;

        // El reposo se toma al empezar, con el celular como lo agarra el jugador.
        canasta.Calibrar();
        StartCoroutine(EmpezarConDemora());
    }

    private IEnumerator EmpezarConDemora()
    {
        if (indicacionInicio != null) indicacionInicio.SetActive(true);
        yield return new WaitForSeconds(segundosAntesDeEmpezar);
        if (indicacionInicio != null) indicacionInicio.SetActive(false);
        spawner.Arrancar();
    }

    private void AlAtrapar(ObjetoQueCae objeto)
    {
        if (terminado) return;

        if (objeto.EsBueno)
        {
            puntos++;
            textoPuntos.SetText("{0}", puntos);
            SoundPlayer.Reproducir(sonidoAtrapar);
            // La dificultad depende del puntaje, no del tiempo, igual que en el vuelo.
            spawner.ActualizarDificultad(puntos);
        }
        else
        {
            SoundPlayer.Reproducir(sonidoMalo);
            PerderVida();
        }
    }

    private void AlCaerseBueno()
    {
        if (terminado) return;

        // Dejar caer un bueno tambien cuesta una vida: si no, quedarse quieto en un rincon esquivando
        // lo malo seria la mejor estrategia, y el juego es atrapar. Los malos que caen no restan.
        SoundPlayer.Reproducir(sonidoSeCayo);
        PerderVida();
    }

    private void PerderVida()
    {
        vidas--;
        MostrarVidas();
        Vibracion.Vibrar();

        if (vidas <= 0) Perder();
    }

    private void MostrarVidas()
    {
        if (textoVidas != null) textoVidas.SetText("{0}", vidas);
        for (int i = 0; i < corazones.Length; i++)
        {
            corazones[i].SetActive(i < vidas);
        }
    }

    private void Perder()
    {
        terminado = true;
        spawner.Frenar();
        // Apagar la canasta suelta el touch y el acelerometro (lo hace su OnDisable).
        canasta.enabled = false;
        StartCoroutine(TerminarConDemora());
    }

    private IEnumerator TerminarConDemora()
    {
        yield return new WaitForSeconds(segundosAntesDelResultado);

        // Lineal con techo, para que una partida muy larga no rompa la economia.
        int monedas = Mathf.Min(puntos * monedasPorPunto, maximoMonedas);
        Finished?.Invoke(new MinigameResult(puntos, monedas, true, puntos >= puntosVictoria));
    }
}
