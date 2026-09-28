using System;
using UnityEngine;

/// La billetera: cuantas monedas y gemas tiene el jugador. Avisa por evento cuando cambia el saldo,
/// asi los contadores de la UI no tienen que preguntar en cada frame.
public class Wallet : MonoBehaviour
{
    [Header("Saldo inicial")]
    [SerializeField] private int monedasIniciales = 100;
    [SerializeField] private int gemasIniciales = 5;

    public event Action<CurrencyType, int> SaldoCambiado;

    // Monedas y gemas, en el orden del enum. Estatico para que sobreviva al ir a un minijuego y
    // volver (la casa se recarga). Al abrir la app lo llena el guardado.
    private static readonly int[] saldo = new int[2];
    private static bool empezado;

    // Para el guardado. null si la billetera todavia no arranco en esta sesion: no hay nada que guardar.
    public static int[] Foto()
    {
        return empezado ? (int[])saldo.Clone() : null;
    }

    public static void Restaurar(int[] guardado)
    {
        if (guardado == null || guardado.Length != saldo.Length) return;
        Array.Copy(guardado, saldo, saldo.Length);
        // Ya tiene saldo: que Empezar no lo pise con el inicial.
        empezado = true;
    }

    public int Obtener(CurrencyType tipo)
    {
        Empezar();
        return saldo[(int)tipo];
    }

    public void Sumar(CurrencyType tipo, int cantidad)
    {
        if (cantidad <= 0) return;
        Empezar();
        saldo[(int)tipo] += cantidad;
        SaldoCambiado?.Invoke(tipo, saldo[(int)tipo]);
    }

    // Se carga el saldo inicial al primer uso y no en Awake: los Awake de cada objeto corren en
    // cualquier orden, y los contadores leen el saldo en su OnEnable, que puede llegar antes.
    private void Empezar()
    {
        if (empezado) return;
        empezado = true;
        saldo[(int)CurrencyType.Monedas] = monedasIniciales;
        saldo[(int)CurrencyType.Gemas] = gemasIniciales;
    }
}
