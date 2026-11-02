using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// Reglas de Camino a casa, un Simon espacial: se muestra un camino desde la casa de Poku hasta la
/// del amigo y hay que repetirlo tocando las celdas en orden. Cada camino completo sube un nivel.
public class CaminoGame : MonoBehaviour, IMinigame
{
    [Header("Referencias")]
    [SerializeField] private GrillaCamino grilla;
    [SerializeField] private SpriteRenderer casaPoku;
    [SerializeField] private SpriteRenderer casaAmigo;
    [SerializeField] private TMP_Text textoNivel;
    [SerializeField] private TMP_Text textoMonedas;

    [Header("Dificultad")]
    // Pasos a tocar en el primer nivel; cada nivel suma uno.
    [SerializeField] private int largoBase = 3;
    // El tiempo por paso baja con el nivel, pero el camino crece: el total para memorizar sigue
    // subiendo. Con un tiempo fijo los niveles altos serian imposibles.
    [SerializeField] private float segundosPorPasoMaximo = 0.6f;
    [SerializeField] private float segundosPorPasoMinimo = 0.35f;
    [SerializeField] private int nivelesHastaMinimo = 15;
    // Las caminatas al azar a veces se encierran en una esquina: se prueba de nuevo hasta este limite.
    [SerializeField] private int intentosPorCamino = 500;

    [Header("Tiempos")]
    [SerializeField] private float pausaAntesDeMostrar = 0.6f;
    [SerializeField] private float pausaEntreNiveles = 0.8f;
    // Pausa entre el error y el resultado, para que se vea cual era la celda correcta.
    [SerializeField] private float segundosAntesDelResultado = 1f;

    [Header("Recompensa")]
    [SerializeField] private int monedasPorNivel = 3;

    [Header("Victoria")]
    // Se gana al completar esta cantidad de caminos. Ademas pone un techo: en una grilla de 4x5 los
    // caminos muy largos ya casi no entran.
    [SerializeField] private int nivelesVictoria = 10;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoAcierto;
    [SerializeField] private AudioClip sonidoNivel;
    [SerializeField] private AudioClip sonidoError;

    public event Action<MinigameResult> Finished;

    private static readonly Vector2Int[] Direcciones =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    // camino[0] es la casa de Poku y no se toca; el jugador repite desde camino[1] hasta el final,
    // que es la casa del amigo.
    private readonly List<Vector2Int> camino = new List<Vector2Int>();
    private readonly List<Vector2Int> vecinosLibres = new List<Vector2Int>(4);
    private bool[,] visitadas;

    // Caminos completos en esta partida. Es tambien el puntaje.
    private int nivel;
    private int monedas;
    private int siguientePaso;
    // Apagado mientras se muestra el camino: un toque sin querer haria perder antes de empezar.
    private bool inputHabilitado;

    public void StartGame()
    {
        nivel = 0;
        monedas = 0;
        textoNivel.SetText("{0}", nivel + 1);
        textoMonedas.SetText("{0}", monedas);

        grilla.Construir();
        grilla.CeldaTocada += AlTocarCelda;
        visitadas = new bool[grilla.Columnas, grilla.Filas];

        StartCoroutine(JugarNivel(pausaAntesDeMostrar));
    }

    private IEnumerator JugarNivel(float pausaInicial)
    {
        inputHabilitado = false;
        yield return new WaitForSeconds(pausaInicial);

        grilla.PintarTodas(CeldaCamino.Estado.Normal);
        GenerarCamino(largoBase + nivel);
        grilla.Acomodar(casaPoku.transform, casaPoku, camino[0]);
        grilla.Acomodar(casaAmigo.transform, casaAmigo, camino[camino.Count - 1]);

        // Cuanto mas avanzado el nivel, menos tiempo por paso, hasta llegar al minimo.
        float segundosPorPaso = Mathf.Lerp(segundosPorPasoMaximo, segundosPorPasoMinimo,
            (float)nivel / Mathf.Max(1, nivelesHastaMinimo));
        WaitForSeconds esperaPaso = new WaitForSeconds(segundosPorPaso);

        // Una espera antes del primer paso, para que se vea donde quedaron las casas.
        yield return esperaPaso;
        for (int i = 1; i < camino.Count; i++)
        {
            CeldaCamino celda = grilla.Celda(camino[i]);
            celda.CambiarEstado(CeldaCamino.Estado.Resaltada);
            yield return esperaPaso;
            celda.CambiarEstado(CeldaCamino.Estado.Normal);
        }

        siguientePaso = 1;
        inputHabilitado = true;
    }

    private void GenerarCamino(int pasos)
    {
        // Si con ningun intento sale un camino de ese largo, se acepta uno mas corto antes que trabar el juego.
        int maximoPasos = grilla.Columnas * grilla.Filas - 1;
        for (int largo = Mathf.Min(pasos, maximoPasos); largo > 0; largo--)
        {
            for (int intento = 0; intento < intentosPorCamino; intento++)
            {
                if (IntentarCaminata(largo)) return;
            }
        }
    }

    // Caminata al azar sin diagonales y sin pisar dos veces la misma celda. Devuelve false si se
    // encierra antes de llegar al largo pedido.
    private bool IntentarCaminata(int pasos)
    {
        Array.Clear(visitadas, 0, visitadas.Length);
        camino.Clear();

        Vector2Int actual = new Vector2Int(UnityEngine.Random.Range(0, grilla.Columnas),
            UnityEngine.Random.Range(0, grilla.Filas));
        camino.Add(actual);
        visitadas[actual.x, actual.y] = true;

        for (int i = 0; i < pasos; i++)
        {
            vecinosLibres.Clear();
            foreach (Vector2Int direccion in Direcciones)
            {
                Vector2Int vecino = actual + direccion;
                if (grilla.EstaAdentro(vecino) && !visitadas[vecino.x, vecino.y]) vecinosLibres.Add(vecino);
            }
            if (vecinosLibres.Count == 0) return false;

            actual = vecinosLibres[UnityEngine.Random.Range(0, vecinosLibres.Count)];
            camino.Add(actual);
            visitadas[actual.x, actual.y] = true;
        }
        return true;
    }

    private void AlTocarCelda(CeldaCamino celda)
    {
        if (!inputHabilitado) return;

        if (celda.Posicion != camino[siguientePaso])
        {
            Perder(celda);
            return;
        }

        celda.CambiarEstado(CeldaCamino.Estado.Acertada);
        SoundPlayer.Reproducir(sonidoAcierto);
        siguientePaso++;

        if (siguientePaso == camino.Count) CompletarNivel();
    }

    private void CompletarNivel()
    {
        inputHabilitado = false;
        nivel++;
        monedas += monedasPorNivel;
        textoMonedas.SetText("{0}", monedas);
        SoundPlayer.Reproducir(sonidoNivel);

        if (nivel >= nivelesVictoria)
        {
            StartCoroutine(Terminar(true));
            return;
        }

        textoNivel.SetText("{0}", nivel + 1);
        StartCoroutine(JugarNivel(pausaEntreNiveles));
    }

    private void Perder(CeldaCamino celdaTocada)
    {
        inputHabilitado = false;
        celdaTocada.CambiarEstado(CeldaCamino.Estado.Error);
        // Se muestra cual era la correcta, para que el error se entienda.
        grilla.Celda(camino[siguientePaso]).CambiarEstado(CeldaCamino.Estado.Resaltada);
        SoundPlayer.Reproducir(sonidoError);
        Vibracion.Vibrar();
        StartCoroutine(Terminar(false));
    }

    private IEnumerator Terminar(bool gano)
    {
        yield return new WaitForSeconds(segundosAntesDelResultado);
        Finished?.Invoke(new MinigameResult(nivel, monedas, true, gano));
    }
}
