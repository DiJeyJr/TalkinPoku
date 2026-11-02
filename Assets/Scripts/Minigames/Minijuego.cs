using System;
using System.Collections;
using UnityEngine;

// Lo que tienen en comun los seis minijuegos. MinigameFlow lo arranca y espera el aviso de que
// termino sin saber cual es, asi el flujo de entrar, mostrar el resultado y volver se escribe una vez.
public abstract class Minijuego : MonoBehaviour
{
    public event Action<MinigameResult> Finished;

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
