using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// ============================================================================
// ScreenStack.cs
// QUE HACE: abre y cierra las pantallas de menu (tienda, minijuegos, dibujo,
//           bandejas, admin) como una pila: volver siempre cierra la ultima que
//           se abrio. Mientras hay una pantalla abierta, apaga el input de la
//           casa (swipe, toques sobre Poku) para que no se active por debajo.
//           El boton atras de Android cierra la pantalla de arriba.
// DONDE ESTA: en "Canvas/Pantallas" de SampleScene.
// PARA CAMBIAR: que scripts se apagan con una pantalla abierta -> lista
//               "Input Del Mundo" en el Inspector.
// USO DESDE OTRO SCRIPT: pantallas.Abrir(objetoPantalla); pantallas.Cerrar();
// ============================================================================

// Abre y cierra las pantallas de menu (tienda, minijuegos) como una pila: volver siempre cierra
// la ultima que se abrio. El boton atras de Android hace lo mismo que el boton de volver.
public class ScreenStack : MonoBehaviour
{
    [Header("Referencias")]
    // Los scripts de la casa leen el touch directo y no saben que hay UI encima. Si no se apagan,
    // un swipe sobre la tienda cambia de habitacion por debajo.
    [SerializeField] private Behaviour[] inputDelMundo;

    // Las pantallas abiertas; la de arriba es la que se ve.
    private readonly Stack<GameObject> pila = new Stack<GameObject>();
    private InputAction volver;

    private void Start()
    {
        // En Android el boton atras llega como la tecla Escape, y Escape ya esta en la accion
        // Cancel del mapa UI. Leyendo la accion anda igual en el celular, con teclado y con joystick.
        volver = InputSystem.actions.FindAction("UI/Cancel");
    }

    // Si se apreto atras/Escape, cierra la pantalla de arriba.
    private void Update()
    {
        if (volver != null && volver.WasPressedThisFrame()) Cerrar();
    }

    // Abre una pantalla encima de la actual.
    public void Abrir(GameObject pantalla)
    {
        if (pila.Count > 0 && pila.Peek() == pantalla) return;

        // Se desactiva la de abajo en vez de dejarla tapada: si queda activa sigue recibiendo
        // toques y se dibuja igual (overdraw).
        if (pila.Count > 0) pila.Peek().SetActive(false);
        else ActivarMundo(false);

        pila.Push(pantalla);
        pantalla.SetActive(true);
    }

    // Cierra la pantalla de arriba y vuelve a la anterior (o a la casa).
    public void Cerrar()
    {
        // Con la pila vacia estamos en la casa: el atras no hace nada, asi no se cierra el juego
        // por accidente.
        if (pila.Count == 0) return;

        pila.Pop().SetActive(false);

        if (pila.Count > 0) pila.Peek().SetActive(true);
        else ActivarMundo(true);
    }

    // Prende o apaga los scripts de input de la casa.
    private void ActivarMundo(bool activo)
    {
        foreach (Behaviour componente in inputDelMundo)
        {
            if (componente != null) componente.enabled = activo;
        }
    }
}
