using System;
using TMPro;
using UnityEngine;

/// Reglas del minijuego de vuelo: un punto por obstaculo que se pasa, y se pierde al chocar.
public class VueloGame : MonoBehaviour, IMinigame
{
    [Header("Referencias")]
    [SerializeField] private Volador volador;
    [SerializeField] private ObstaculoSpawner spawner;
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private GameObject indicacionInicio;

    [Header("Recompensa")]
    [SerializeField] private int monedasPorPunto = 1;
    [SerializeField] private int maximoMonedas = 50;

    [Header("Sonidos")]
    // Cada obstaculo pasado es una moneda ganada.
    [SerializeField] private AudioClip sonidoPunto;

    public event Action<MinigameResult> Finished;

    private int puntos;

    public void StartGame()
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
    }

    private void Perder()
    {
        spawner.Frenar();
        // Lineal con techo, para que una partida muy larga no rompa la economia.
        int monedas = Mathf.Min(puntos * monedasPorPunto, maximoMonedas);
        Finished?.Invoke(new MinigameResult(puntos, monedas, true));
    }
}
