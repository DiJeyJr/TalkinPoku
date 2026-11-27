using System;
using UnityEngine;
using UnityEngine.InputSystem;

// ============================================================================
// ShakeDetector.cs
// QUE HACE: detecta cuando se sacude el celular (con el acelerometro) y avisa.
// DONDE ESTA: en el objeto "Minijuego" de las escenas Cocina y Limpieza.
// PARA CAMBIAR: que tan fuerte hay que sacudir -> shakeThreshold (mas alto =
//               hay que sacudir mas fuerte). Cada cuanto puede contar otra
//               sacudida -> shakeCooldown.
// AVISA: el evento Sacudido. Que pasa al sacudir lo decide el minijuego.
// ============================================================================

// Detecta una sacudida del celular como en la clase 5: compara la aceleracion cruda de este frame
// con la del anterior. No suaviza: suavizar borraria justo el pico que se quiere detectar.
// Avisa por evento; que significa sacudir (mezclar, despertar) lo decide otro script.
public class ShakeDetector : MonoBehaviour
{
    [Header("Shake")]
    // [AJUSTE] Cuanto tiene que cambiar la aceleracion de un frame al otro para contar como sacudida.
    [SerializeField] private float shakeThreshold = 2f;
    // [AJUSTE] Segundos de espera minima entre dos sacudidas.
    [SerializeField] private float shakeCooldown = 0.8f;

    public event Action Sacudido;

    private Vector3 rawAcceleration;
    private Vector3 previousAcceleration;
    private bool initialized;
    // Momento a partir del cual puede contar la proxima sacudida.
    private float nextShakeTime;

    private void OnEnable()
    {
        // Los sensores vienen apagados en el Input System: sin esto ReadValue da siempre cero.
        if (Accelerometer.current != null) InputSystem.EnableDevice(Accelerometer.current);
        initialized = false;
    }

    private void OnDisable()
    {
        // Se apaga al salir para no gastar bateria con un sensor que nadie lee. Sin contador: la
        // Canasta de Atrapar es el otro que lo usa y nunca estan en la misma escena.
        if (Accelerometer.current != null) InputSystem.DisableDevice(Accelerometer.current);
    }

    // Cada frame lee el acelerometro y lo compara con el frame anterior.
    private void Update()
    {
        // En el Editor sin Unity Remote no hay acelerometro: no se detecta nada y listo.
        if (Accelerometer.current == null) return;

        rawAcceleration = Accelerometer.current.acceleration.ReadValue();

        if (!initialized)
        {
            previousAcceleration = rawAcceleration;
            initialized = true;
        }

        CheckShake();

        // Al final del Update: si se actualizara antes de comparar, la diferencia daria siempre cero.
        previousAcceleration = rawAcceleration;
    }

    // Si el cambio supera el umbral y ya paso el cooldown, avisa la sacudida.
    private void CheckShake()
    {
        // Magnitud de la resta y no resta de magnitudes: un giro rapido cambia la direccion sin
        // cambiar el largo, y con la resta de magnitudes no se veria.
        float delta = (rawAcceleration - previousAcceleration).magnitude;

        if (delta >= shakeThreshold && Time.time >= nextShakeTime)
        {
            nextShakeTime = Time.time + shakeCooldown;
            Sacudido?.Invoke();
        }
    }
}
