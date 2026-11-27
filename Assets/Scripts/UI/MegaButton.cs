using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ============================================================================
// MegaButton.cs
// QUE HACE: el boton de la Mega Evolucion, que a la vez es su barra. Se llena
//           con la Energia Mega; lleno late y se puede tocar para activarla;
//           activa, muestra el tiempo que queda. Antes del nivel de la Mega no
//           se ve.
// DONDE ESTA: en "Canvas/SafeArea/HUD/Mega" de SampleScene.
// PARA CAMBIAR: cada cuanto late cuando esta lista -> segundosEntreLatidos.
//               los textos ("¡MEGA!", porcentaje, minutos) -> Refrescar().
//               colores del texto -> colorSobreGris / colorSobreDorado.
// ============================================================================

// El boton de la Mega Evolucion, que es tambien su barra: se va llenando con la Energia Mega, con
// la barra llena late y se puede tocar, y mientras dura Mega-Pokuchan se vacia con el tiempo que
// le queda. Antes del nivel 50 no se muestra. La Image de relleno tiene que estar en Filled.
public class MegaButton : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private MegaEvolution mega;
    [SerializeField] private Progression progreso;
    // Lo que se esconde antes de desbloquearla. No puede ser este mismo objeto: si se apaga, deja
    // de escuchar la subida al nivel 50.
    [SerializeField] private GameObject contenido;
    [SerializeField] private Button boton;
    [SerializeField] private Image relleno;
    [SerializeField] private TMP_Text texto;

    [Header("Feedback")]
    // Opcional: late al cargarse y, con la barra llena, cada tanto para que se note que esta lista.
    [SerializeField] private Latido latido;
    // [AJUSTE] Segundos entre latidos cuando esta lista.
    [SerializeField] private float segundosEntreLatidos = 1.5f;
    // El texto queda del lado derecho, que es el ultimo en llenarse: casi siempre esta sobre el gris
    // y ahi el oscuro no se lee. Pasa al oscuro cuando el dorado ya llego abajo del texto.
    [SerializeField] private Color colorSobreGris = new Color(1f, 0.95f, 0.85f);
    [SerializeField] private Color colorSobreDorado = new Color(0.24f, 0.14f, 0.3f);
    [SerializeField] private float llenoBajoElTexto = 0.8f;

    private float proximoLatido;

    private void Awake()
    {
        // Tocar el boton activa la Mega.
        boton.onClick.AddListener(mega.Activar);
    }

    private void OnEnable()
    {
        mega.EnergiaCambiada += AlCambiarEnergia;
        mega.MegaCambiada += AlCambiarMega;
        // XpCambiada y no NivelSubido: tambien avisa cuando el admin reinicia al nivel 1.
        progreso.XpCambiada += AlCambiarXp;
    }

    private void OnDisable()
    {
        mega.EnergiaCambiada -= AlCambiarEnergia;
        mega.MegaCambiada -= AlCambiarMega;
        progreso.XpCambiada -= AlCambiarXp;
    }

    private void Start()
    {
        Refrescar();
    }

    // Con la barra llena, late cada tanto.
    private void Update()
    {
        if (!mega.Lista || Time.time < proximoLatido) return;
        proximoLatido = Time.time + segundosEntreLatidos;
        if (latido != null) latido.Latir();
    }

    // Actualiza visibilidad, relleno, si se puede tocar y el texto.
    private void Refrescar()
    {
        contenido.SetActive(mega.Desbloqueada);
        relleno.fillAmount = mega.Fraccion;
        boton.interactable = mega.Lista;
        texto.color = mega.Fraccion >= llenoBajoElTexto ? colorSobreDorado : colorSobreGris;

        if (MegaEvolution.Activa)
        {
            // Activa: minutos:segundos que quedan.
            int segundos = Mathf.CeilToInt((float)mega.SegundosRestantes);
            texto.SetText("{0}:{1:00}", segundos / 60, segundos % 60);
        }
        else if (mega.Lista) texto.SetText("¡MEGA!");
        else texto.SetText("{0}%", Mathf.FloorToInt(mega.Fraccion * 100f));
    }

    private void AlCambiarEnergia(float fraccion)
    {
        bool cargando = !MegaEvolution.Activa;
        Refrescar();
        if (cargando && latido != null) latido.Latir();
    }

    private void AlCambiarMega(bool activa)
    {
        Refrescar();
    }

    private void AlCambiarXp(int xp, int requerida)
    {
        Refrescar();
    }
}
