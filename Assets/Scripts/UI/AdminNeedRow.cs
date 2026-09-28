using TMPro;
using UnityEngine;

/// Una fila del panel de admin: muestra cuanto tiene una necesidad y la sube o baja de a saltos.
public class AdminNeedRow : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private NeedsSystem necesidades;
    [SerializeField] private TMP_Text textoValor;

    [Header("Necesidad")]
    [SerializeField] private NeedType tipo;

    [Header("Valores")]
    [SerializeField] private float salto = 10f;

    private int mostrado = -1;

    private void OnEnable()
    {
        necesidades.NeedChanged += AlCambiar;
        Mostrar(necesidades.Obtener(tipo));
    }

    private void OnDisable()
    {
        necesidades.NeedChanged -= AlCambiar;
    }

    // Las llaman los botones - y + de la fila desde su OnClick. La higiene se mide en manchas, asi
    // que esa fila la mueve de a una mancha, igual que la esponja.
    public void Subir()
    {
        if (tipo == NeedType.Higiene) necesidades.LimpiarMancha();
        else necesidades.Sumar(tipo, salto);
    }

    public void Bajar()
    {
        if (tipo == NeedType.Higiene) necesidades.AgregarMancha();
        else necesidades.Sumar(tipo, -salto);
    }

    private void AlCambiar(NeedType cambiada, float valor)
    {
        if (cambiada == tipo) Mostrar(valor);
    }

    private void Mostrar(float valor)
    {
        // El valor baja un poquito en cada frame: el texto se reescribe solo cuando cambia el entero.
        int entero = Mathf.RoundToInt(valor);
        if (entero == mostrado) return;
        mostrado = entero;
        textoValor.SetText("{0}", entero);
    }
}
