using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ============================================================================
// LevelBar.cs
// QUE HACE: el nivel en el HUD: muestra el numero de nivel dentro de la
//           estrella y la va llenando con la XP que lleva para el proximo nivel.
// DONDE ESTA: en "Canvas/SafeArea/HUD/Nivel" de SampleScene.
// PARA CAMBIAR: velocidad del llenado -> duracionLlenado.
//               desde/hasta donde se llena el dibujo -> rellenoDesde / rellenoHasta.
// ============================================================================

// El nivel en el HUD: el numero adentro de la estrella, que se va llenando con la experiencia que
// falta para el proximo. La Image de relleno tiene que estar en Image Type = Filled.
public class LevelBar : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Progression progreso;
    [SerializeField] private TMP_Text textoNivel;
    [SerializeField] private Image relleno;

    [Header("Relleno")]
    // [AJUSTE] Parte del sprite que se llena (0 a 1).
    // Que parte del sprite ocupa el dibujo: el PNG tiene margen transparente, y sin esto con poca
    // experiencia la estrella se veria vacia y casi al final ya se veria llena.
    [SerializeField] private float rellenoDesde = 0f;
    [SerializeField] private float rellenoHasta = 1f;

    [Header("Feedback")]
    // Opcional: late al subir de nivel.
    [SerializeField] private Latido latido;
    // [AJUSTE] Segundos que tarda en llenarse hasta el valor nuevo.
    [SerializeField] private float duracionLlenado = 0.4f;

    private float mostrado;
    private float objetivo;
    private float velocidad;

    private void OnEnable()
    {
        progreso.XpCambiada += AlCambiarXp;
        progreso.NivelSubido += AlSubir;
    }

    private void OnDisable()
    {
        progreso.XpCambiada -= AlCambiarXp;
        progreso.NivelSubido -= AlSubir;
    }

    private void Start()
    {
        // Al arrancar se muestra como esta, sin llenarse.
        textoNivel.SetText("{0}", progreso.Nivel);
        objetivo = Fraccion(progreso.Xp, progreso.XpRequerida);
        mostrado = objetivo;
        Pintar();
    }

    // Cada frame acerca el relleno al objetivo.
    private void Update()
    {
        if (mostrado == objetivo) return;
        mostrado = Mathf.MoveTowards(mostrado, objetivo, velocidad * Time.deltaTime);
        Pintar();
    }

    private void Pintar()
    {
        relleno.fillAmount = Mathf.Lerp(rellenoDesde, rellenoHasta, mostrado);
    }

    // Subio de nivel: cambia el numero y vacia la estrella.
    private void AlSubir(int nivel)
    {
        textoNivel.SetText("{0}", nivel);
        // Arranca de cero en el nivel nuevo, no baja desde llena.
        mostrado = 0f;
        Pintar();
        if (latido != null) latido.Latir();
    }

    private void AlCambiarXp(int xp, int requerida)
    {
        objetivo = Fraccion(xp, requerida);
        // Tarda siempre lo mismo, como el contador de monedas: la velocidad sale de la diferencia.
        velocidad = Mathf.Abs(objetivo - mostrado) / duracionLlenado;
    }

    // XP actual / XP necesaria, de 0 a 1.
    private static float Fraccion(int xp, int requerida)
    {
        // En el nivel maximo no hay proximo: la barra queda llena.
        return requerida <= 0 ? 1f : Mathf.Clamp01((float)xp / requerida);
    }
}
