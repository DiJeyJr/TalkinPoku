using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Reglas de Unir a los personajes: arma cada nivel, cobra una vida por cada trazo fallido y paga
// monedas por los que llegan.
public class UnirGame : Minijuego
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;
    [SerializeField] private TrazoUnir trazo;
    [SerializeField] private Collider2D poku;
    [SerializeField] private Collider2D amigo;
    // Se reusan en vez de instanciar: alcanza con tener tantos como el nivel que mas usa.
    [SerializeField] private Transform[] obstaculos;
    [SerializeField] private TMP_Text textoNivel;
    [SerializeField] private TMP_Text textoVidas;
    [SerializeField] private TMP_Text textoMonedas;

    [Header("Niveles")]
    [SerializeField] private NivelUnir[] niveles;
    [SerializeField] private int vidasIniciales = 3;

    [Header("Recompensa")]
    [SerializeField] private int monedasPorNivel = 3;
    // Se cobra entero si sobro toda la tinta; si no, en proporcion a lo que sobro.
    [SerializeField] private int bonusMaximoTinta = 3;

    [Header("Feedback")]
    [SerializeField] private float velocidadCaminata = 3f;
    [SerializeField] private float segundosMostrarFallo = 0.6f;
    [SerializeField] private float duracionFestejo = 0.6f;
    [SerializeField] private float alturaFestejo = 0.4f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip sonidoUnio;
    [SerializeField] private AudioClip sonidoFallo;
    [SerializeField] private AudioClip sonidoMoneda;

    private int nivelActual;
    private int vidas;
    private int monedas;
    private BoxCollider2D[] cajas;
    private SpriteRenderer[] dibujos;

    private void Awake()
    {
        if (camara == null) camara = Camera.main;

        cajas = new BoxCollider2D[obstaculos.Length];
        dibujos = new SpriteRenderer[obstaculos.Length];
        for (int i = 0; i < obstaculos.Length; i++)
        {
            cajas[i] = obstaculos[i].GetComponent<BoxCollider2D>();
            dibujos[i] = obstaculos[i].GetComponentInChildren<SpriteRenderer>(true);
        }
    }

    public override void StartGame()
    {
        nivelActual = 0;
        vidas = vidasIniciales;
        monedas = 0;
        textoVidas.SetText("{0}", vidas);
        textoMonedas.SetText("{0}", monedas);

        trazo.Unio += AlUnir;
        trazo.Fallo += AlFallar;

        ArmarNivel();
    }

    private void OnDestroy()
    {
        if (trazo == null) return;
        trazo.Unio -= AlUnir;
        trazo.Fallo -= AlFallar;
    }

    private void ArmarNivel()
    {
        NivelUnir nivel = niveles[nivelActual];
        textoNivel.SetText("{0}/{1}", nivelActual + 1, niveles.Length);

        poku.transform.position = AMundo(nivel.PosicionPoku);
        amigo.transform.position = AMundo(nivel.PosicionAmigo);

        ObstaculoUnir[] datos = nivel.Obstaculos;
        for (int i = 0; i < obstaculos.Length; i++)
        {
            bool usado = datos != null && i < datos.Length;
            obstaculos[i].gameObject.SetActive(usado);
            if (!usado) continue;

            Vector2 tamano = datos[i].tamano;
            float rotacion = datos[i].rotacion;
            // El dibujo es un cerco acostado: uno parado se arma acostado y girado, si no los postes
            // quedarian de costado.
            if (tamano.y > tamano.x)
            {
                tamano = new Vector2(tamano.y, tamano.x);
                rotacion += 90f;
            }

            obstaculos[i].position = AMundo(datos[i].posicion);
            obstaculos[i].rotation = Quaternion.Euler(0f, 0f, rotacion);
            cajas[i].size = tamano;
            AjustarDibujo(dibujos[i], tamano);
        }
        // Los colliders movidos por Transform se actualizan recien en el proximo paso de fisica; sin
        // esto el primer Linecast del nivel podria ver los obstaculos donde estaban antes.
        Physics2D.SyncTransforms();

        trazo.Habilitar(nivel.Tinta, poku, amigo);
    }

    // Escalar el cerco hasta el tamano pedido lo estira. En cambio se achica parejo hasta el grosor
    // del obstaculo y el largo se completa repitiendo el dibujo (Draw Mode Tiled).
    private void AjustarDibujo(SpriteRenderer dibujo, Vector2 tamano)
    {
        float altoSprite = dibujo.sprite.bounds.size.y;
        float escala = tamano.y / altoSprite;
        dibujo.transform.localScale = new Vector3(escala, escala, 1f);
        dibujo.size = new Vector2(tamano.x / escala, altoSprite);
    }

    private void AlFallar()
    {
        vidas--;
        textoVidas.SetText("{0}", vidas);
        SoundPlayer.Reproducir(sonidoFallo);
        Vibracion.Vibrar();
        StartCoroutine(DespuesDelFallo());
    }

    private IEnumerator DespuesDelFallo()
    {
        yield return new WaitForSeconds(segundosMostrarFallo);

        if (vidas <= 0) Terminar();
        else trazo.Habilitar(niveles[nivelActual].Tinta, poku, amigo);
    }

    private void AlUnir(Collider2D desde, Collider2D hasta)
    {
        SoundPlayer.Reproducir(sonidoUnio);
        StartCoroutine(Caminar(desde.transform));
    }

    private IEnumerator Caminar(Transform caminante)
    {
        List<Vector3> puntos = trazo.Puntos;
        // MoveTowards nunca se pasa del objetivo, asi que el personaje pisa cada vertice de la linea.
        for (int i = 0; i < puntos.Count; i++)
        {
            while (caminante.position != puntos[i])
            {
                caminante.position = Vector3.MoveTowards(caminante.position, puntos[i], velocidadCaminata * Time.deltaTime);
                yield return null;
            }
        }

        yield return Festejar(caminante);

        monedas += monedasPorNivel + Mathf.RoundToInt(bonusMaximoTinta * trazo.TintaRestante);
        textoMonedas.SetText("{0}", monedas);
        SoundPlayer.Reproducir(sonidoMoneda);

        nivelActual++;
        if (nivelActual >= niveles.Length) Terminar();
        else ArmarNivel();
    }

    private IEnumerator Festejar(Transform personaje)
    {
        // El valor absoluto del seno da dos saltitos que terminan justo en el piso.
        Vector3 piso = personaje.position;
        float t = 0f;
        while (t < duracionFestejo)
        {
            t += Time.deltaTime;
            float salto = Mathf.Abs(Mathf.Sin(t / duracionFestejo * Mathf.PI * 2f)) * alturaFestejo;
            personaje.position = piso + Vector3.up * salto;
            yield return null;
        }
        personaje.position = piso;
    }

    private void Terminar()
    {
        trazo.Deshabilitar();
        bool completoTodos = nivelActual >= niveles.Length;
        Terminar(new MinigameResult(nivelActual, monedas, true, completoTodos));
    }

    private Vector3 AMundo(Vector2 fraccionPantalla)
    {
        Vector3 mundo = camara.ViewportToWorldPoint(fraccionPantalla);
        mundo.z = 0f;
        return mundo;
    }
}
