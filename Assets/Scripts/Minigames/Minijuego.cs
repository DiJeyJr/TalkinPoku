using System;
using System.Collections;
using UnityEngine;

// ============================================================================
// Minijuego.cs
// QUE HACE: clase base (abstracta) de los seis minijuegos (Atrapar, Camino,
//           Cocina, Limpieza, Unir, Vuelo). Cada minijuego hereda de esta,
//           implementa StartGame() y cuando termina llama a Terminar(resultado).
//           MinigameFlow escucha el evento Finished sin saber cual minijuego es.
// DONDE ESTA: no se pone sola; cada XxxGame hereda de ella.
// PARA HACER UN MINIJUEGO NUEVO: crear una clase que herede de Minijuego,
//   implementar StartGame() y llamar Terminar(new MinigameResult(...)) al final.
//   Ponerla en el objeto "Minijuego" de la escena junto a MinigameFlow.
// ============================================================================

// Lo que tienen en comun los seis minijuegos. MinigameFlow lo arranca y espera el aviso de que
// termino sin saber cual es, asi el flujo de entrar, mostrar el resultado y volver se escribe una vez.
public abstract class Minijuego : MonoBehaviour
{
    // Se dispara cuando termina la partida, con el resultado.
    public event Action<MinigameResult> Finished;

    // Cada minijuego arranca su partida aca (lo llama MinigameFlow en Start).
    public abstract void StartGame();

    // La demora deja ver como termino (el choque, el lugar limpio) antes de tapar todo con el resultado.
    protected void Terminar(MinigameResult resultado, float demora = 0f)
    {
        StartCoroutine(AvisarDespues(resultado, demora));
    }

    private IEnumerator AvisarDespues(MinigameResult resultado, float demora)
    {
        if (demora > 0f) yield return new WaitForSeconds(demora);
        Finished?.Invoke(resultado);
    }

    // De 0 a 1, cuanto se acerco al tope de dificultad. La curva decide como sube: rapido al
    // principio, de a poco al final, o lo que se ajuste en el Inspector.
    public static float Avance(int valor, int tope, AnimationCurve curva)
    {
        float lineal = tope > 0 ? Mathf.Clamp01((float)valor / tope) : 1f;
        return Mathf.Clamp01(curva.Evaluate(lineal));
    }
}
