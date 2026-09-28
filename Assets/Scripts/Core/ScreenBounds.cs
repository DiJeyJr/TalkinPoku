using UnityEngine;

/// Paredes invisibles en los bordes de la pantalla y un piso, para que la pelota rebote contra
/// todos lados. Se arman al arrancar con lo que ve la camara, asi andan en cualquier celular.
[RequireComponent(typeof(EdgeCollider2D))]
public class ScreenBounds : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camara;

    [Header("Medidas")]
    // En unidades de mundo: la linea donde apoya Poku, no el borde de abajo de la pantalla.
    [SerializeField] private float alturaPiso = -5f;

    private void Start()
    {
        if (camara == null) camara = Camera.main;

        // Camara ortografica: la Z no cambia X e Y, igual que en Draggable.
        Vector2 abajoIzquierda = camara.ViewportToWorldPoint(Vector3.zero);
        Vector2 arribaDerecha = camara.ViewportToWorldPoint(Vector3.one);

        // Un solo EdgeCollider2D que da la vuelta: es una linea sin area, asi rebota la pelota pero no
        // tapa toques (un swipe que empieza cerca del borde sigue cambiando de habitacion).
        Vector2[] puntos =
        {
            new Vector2(abajoIzquierda.x, alturaPiso),
            new Vector2(abajoIzquierda.x, arribaDerecha.y),
            new Vector2(arribaDerecha.x, arribaDerecha.y),
            new Vector2(arribaDerecha.x, alturaPiso),
            new Vector2(abajoIzquierda.x, alturaPiso)
        };

        // Los puntos del collider son locales al objeto.
        for (int i = 0; i < puntos.Length; i++)
        {
            puntos[i] = transform.InverseTransformPoint(puntos[i]);
        }
        GetComponent<EdgeCollider2D>().points = puntos;
    }
}
