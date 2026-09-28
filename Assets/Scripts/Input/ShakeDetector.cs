using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// Detecta una sacudida del celular como en la clase 5: compara la aceleracion cruda de este frame
/// con la del anterior. No suaviza: suavizar borraria justo el pico que se quiere detectar.
/// Avisa por evento; que significa sacudir (mezclar, despertar) lo decide otro script.
public class ShakeDetector : MonoBehaviour
{
    [Header("Shake")]
    [SerializeField] private float shakeThreshold = 2f;
    [SerializeField] private float shakeCooldown = 0.8f;

    public event Action Sacudido;

    private Vector3 rawAcceleration;
    private Vector3 previousAcceleration;
    private bool initialized;
    private float nextShakeTime;

    private void OnEnable()
    {
        // Los sensores vienen apagados en el Input System: sin esto ReadValue da siempre cero.
        if (Accelerometer.current != null) InputSystem.EnableDevice(Accelerometer.current);
        initialized = false;
    }

    private void OnDisable()
    {
        // Se apaga al salir para no gastar bateria con un sensor que nadie lee.
        if (Accelerometer.current != null) InputSystem.DisableDevice(Accelerometer.current);
    }

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
