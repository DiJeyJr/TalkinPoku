using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Panel que aparece al terminar cualquier minijuego: si se gano o se perdio, el puntaje y lo que
// se cobra. No paga ni cambia de escena: lee los numeros de MinigameReward y avisa con eventos
// que boton se apreto, y MinigameFlow decide a donde ir.
public class PantallaResultado : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform tarjeta;
    [SerializeField] private TMP_Text textoTitulo;
    [SerializeField] private TMP_Text textoPuntos;
    [SerializeField] private TMP_Text textoMonedas;
    [SerializeField] private TMP_Text textoXp;
    [SerializeField] private Button botonReintentar;
    [SerializeField] private Button botonVolver;
    // En el minijuego no hay Progression en la escena: el bonus de la Mega y la XP salen del config.
    [SerializeField] private ProgressionConfig progresion;

    [Header("Titulo")]
    [SerializeField] private string tituloVictoria = "¡Ganaste!";
    [SerializeField] private string tituloDerrota = "¡Perdiste!";
    [SerializeField] private Color colorVictoria = new Color(1f, 0.85f, 0.2f);
    [SerializeField] private Color colorDerrota = new Color(0.8f, 0.8f, 0.85f);

    [Header("Animacion")]
    [SerializeField] private float segundosPop = 0.25f;
    [SerializeField] private float segundosContador = 1f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoVictoria;
    [SerializeField] private AudioClip sonidoDerrota;

    public event Action PidioReintentar;
    public event Action PidioVolver;

    private void Awake()
    {
        // Menu, no gameplay: estos botones si pueden usar onClick.
        botonReintentar.onClick.AddListener(() => PidioReintentar?.Invoke());
        botonVolver.onClick.AddListener(() => PidioVolver?.Invoke());
    }

    public void Ocultar()
    {
        gameObject.SetActive(false);
    }

    public void Mostrar(MinigameResult resultado)
    {
        gameObject.SetActive(true);

        int monedas = MinigameReward.MonedasCobradas(resultado, progresion);
        int xp = MinigameReward.XpCobrada(resultado, progresion);

        // El sonido va en el mismo frame que el panel: es lo que le dice al jugador que termino.
        textoTitulo.SetText(resultado.Won ? tituloVictoria : tituloDerrota);
        textoTitulo.color = resultado.Won ? colorVictoria : colorDerrota;
        SoundPlayer.Reproducir(resultado.Won ? sonidoVictoria : sonidoDerrota);

        textoPuntos.SetText("Puntos: {0}", resultado.Score);
        textoMonedas.SetText("Monedas: {0}", 0);
        textoXp.SetText("XP: +{0}", xp);

        // En cero desde el primer frame, si no la tarjeta se ve entera un instante antes del pop.
        tarjeta.localScale = Vector3.zero;
        StartCoroutine(Animar(monedas));
    }

    private IEnumerator Animar(int monedas)
    {
        // Tiempo sin escala: si algun dia se pausa con timeScale 0, el panel igual aparece.
        float t = 0f;
        while (t < segundosPop)
        {
            t += Time.unscaledDeltaTime;
            tarjeta.localScale = Vector3.one * EaseOutBack(Mathf.Clamp01(t / segundosPop));
            yield return null;
        }
        tarjeta.localScale = Vector3.one;

        int mostradas = 0;
        t = 0f;
        while (t < segundosContador && monedas > 0)
        {
            t += Time.unscaledDeltaTime;
            // Frena al llegar para que el ultimo numero se lea.
            float avance = 1f - Mathf.Pow(1f - Mathf.Clamp01(t / segundosContador), 2f);
            int valor = Mathf.RoundToInt(monedas * avance);
            if (valor != mostradas)
            {
                mostradas = valor;
                textoMonedas.SetText("Monedas: {0}", mostradas);
            }
            yield return null;
        }
        textoMonedas.SetText("Monedas: {0}", monedas);
    }

    // Pasa un poco de 1 y vuelve: es el rebote que hace sentir "viva" a la tarjeta.
    private static float EaseOutBack(float x)
    {
        const float sobrepaso = 1.70158f;
        float p = x - 1f;
        return 1f + (sobrepaso + 1f) * p * p * p + sobrepaso * p * p;
    }
}
