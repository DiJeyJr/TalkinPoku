using System;
using UnityEngine;

/// Arma la grilla de celdas por codigo a partir de un prefab, con el tamano que entra en el ancho
/// de la camara. Asi no hay que acomodar veinte objetos a mano y anda en cualquier celular.
public class GrillaCamino : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;
    [SerializeField] private CeldaCamino prefabCelda;

    [Header("Tamano")]
    [SerializeField] private int columnas = 4;
    [SerializeField] private int filas = 5;
    // En unidades de mundo, a cada lado de la pantalla.
    [SerializeField] private float margenLateral = 0.3f;
    // Que parte del alto visible puede ocupar la grilla, para dejar lugar al HUD.
    [SerializeField, Range(0.2f, 1f)] private float altoMaximo = 0.65f;
    // Que parte de la celda ocupa el sprite: el resto queda como separacion entre celdas.
    [SerializeField, Range(0.5f, 1f)] private float relleno = 0.9f;

    public event Action<CeldaCamino> CeldaTocada;

    public int Columnas => columnas;
    public int Filas => filas;

    private CeldaCamino[,] celdas;
    private float tamanoCelda;
    private Vector3 origen;

    public void Construir()
    {
        if (celdas != null) return;
        if (camara == null) camara = Camera.main;

        // Camara ortografica: orthographicSize es la mitad del alto visible.
        float altoVisible = camara.orthographicSize * 2f;
        float anchoVisible = altoVisible * camara.aspect;
        float porAncho = (anchoVisible - margenLateral * 2f) / columnas;
        float porAlto = altoVisible * altoMaximo / filas;
        tamanoCelda = Mathf.Min(porAncho, porAlto);

        // La grilla queda centrada en este objeto: la celda (0, 0) es la de abajo a la izquierda.
        origen = transform.position - new Vector3((columnas - 1) * 0.5f, (filas - 1) * 0.5f, 0f) * tamanoCelda;
        origen.z = 0f;

        celdas = new CeldaCamino[columnas, filas];
        for (int x = 0; x < columnas; x++)
        {
            for (int y = 0; y < filas; y++)
            {
                CeldaCamino celda = Instantiate(prefabCelda, transform);
                Vector2Int posicion = new Vector2Int(x, y);
                Acomodar(celda.transform, celda.GetComponent<SpriteRenderer>(), posicion);
                celda.Inicializar(posicion);
                celda.Tocada += AvisarToque;
                celdas[x, y] = celda;
            }
        }
    }

    // Pone un objeto con sprite en el centro de una celda y lo escala para que ocupe lo mismo que
    // ella. Se usa tambien para las casas, que no son celdas.
    public void Acomodar(Transform objeto, SpriteRenderer sprite, Vector2Int posicion)
    {
        objeto.position = origen + new Vector3(posicion.x, posicion.y, 0f) * tamanoCelda;
        // El sprite puede medir cualquier cosa segun sus pixeles por unidad: se escala por su medida real.
        float medidaSprite = sprite.sprite.bounds.size.x;
        float escala = tamanoCelda * relleno / medidaSprite;
        objeto.localScale = new Vector3(escala, escala, 1f);
    }

    public CeldaCamino Celda(Vector2Int posicion) => celdas[posicion.x, posicion.y];

    public bool EstaAdentro(Vector2Int posicion)
    {
        return posicion.x >= 0 && posicion.x < columnas && posicion.y >= 0 && posicion.y < filas;
    }

    public void PintarTodas(CeldaCamino.Estado estado)
    {
        foreach (CeldaCamino celda in celdas) celda.CambiarEstado(estado);
    }

    private void AvisarToque(CeldaCamino celda) => CeldaTocada?.Invoke(celda);
}
