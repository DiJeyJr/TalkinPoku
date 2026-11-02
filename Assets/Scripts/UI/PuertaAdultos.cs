using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// La puerta para adultos antes de pagar con dinero real: una multiplicacion escrita con palabras y
// varias respuestas para elegir. Un chico que todavia no lee o no sabe las tablas no la pasa; un
// adulto la resuelve en dos segundos. Es lo habitual en apps para chicos (App Store la exige en Kids).
public class PuertaAdultos : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject panel;
    // Que se esta por comprar y cuanto sale, para que el adulto sepa que esta aprobando.
    [SerializeField] private TMP_Text textoCompra;
    [SerializeField] private TMP_Text pregunta;
    // Cada boton con un TMP_Text adentro. Mas botones, menos chance de acertar tocando al azar.
    [SerializeField] private Button[] opciones;
    [SerializeField] private Button cancelar;

    [Header("Cuenta")]
    // Hasta nueve porque los numeros van escritos con palabras.
    [SerializeField] private int factorMinimo = 3;
    [SerializeField] private int factorMaximo = 9;
    // Las respuestas falsas quedan cerca de la verdadera: no se puede adivinar por tamano.
    [SerializeField] private int distanciaMaxima = 10;

    private static readonly string[] palabras =
    {
        "cero", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve"
    };

    private TMP_Text[] textosOpciones;
    private int[] valores;
    private readonly List<int> usados = new List<int>();
    private int respuesta;
    private Action alAprobar;

    private void Awake()
    {
        textosOpciones = new TMP_Text[opciones.Length];
        valores = new int[opciones.Length];
        for (int i = 0; i < opciones.Length; i++)
        {
            int indice = i;
            opciones[i].onClick.AddListener(() => Elegir(indice));
            textosOpciones[i] = opciones[i].GetComponentInChildren<TMP_Text>();
        }
        cancelar.onClick.AddListener(Cerrar);
        panel.SetActive(false);
    }

    private void OnDisable()
    {
        // Igual que el cartel de confirmacion: si se cierra la tienda, la puerta no queda abierta.
        Cerrar();
    }

    public void Pedir(string queSeCompra, Action siAprueba)
    {
        int a = Factor();
        int b = Factor();
        respuesta = a * b;

        textoCompra.SetText(queSeCompra);
        pregunta.SetText("¿Cuánto es " + palabras[a] + " por " + palabras[b] + "?");

        // La correcta en un lugar al azar; en los demas, numeros cercanos y sin repetir.
        int correcta = UnityEngine.Random.Range(0, valores.Length);
        // Con una distancia mas chica que la cantidad de botones no alcanzarian los numeros distintos.
        int distancia = Mathf.Max(distanciaMaxima, valores.Length);
        usados.Clear();
        usados.Add(respuesta);
        for (int i = 0; i < valores.Length; i++)
        {
            if (i == correcta)
            {
                valores[i] = respuesta;
            }
            else
            {
                int falsa;
                do falsa = respuesta + UnityEngine.Random.Range(-distancia, distancia + 1);
                while (falsa <= 0 || usados.Contains(falsa));
                usados.Add(falsa);
                valores[i] = falsa;
            }
            textosOpciones[i].SetText("{0}", valores[i]);
        }

        alAprobar = siAprueba;
        panel.SetActive(true);
    }

    private int Factor()
    {
        int minimo = Mathf.Clamp(factorMinimo, 2, 9);
        int maximo = Mathf.Clamp(factorMaximo, minimo, 9);
        return UnityEngine.Random.Range(minimo, maximo + 1);
    }

    // Si se equivoca se cierra sin cobrar: no hay segundo intento con la misma cuenta.
    private void Elegir(int indice)
    {
        Action accion = valores[indice] == respuesta ? alAprobar : null;
        Cerrar();
        accion?.Invoke();
    }

    private void Cerrar()
    {
        alAprobar = null;
        if (panel != null) panel.SetActive(false);
    }
}
