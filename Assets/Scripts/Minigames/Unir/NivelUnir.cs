using System;
using UnityEngine;

// ============================================================================
// NivelUnir.cs  (minijuego "Unir personajes")
// QUE HACE: describe un nivel de Unir: donde estan Poku y el amigo, cuanta
//           tinta hay y donde estan los obstaculos.
// DONDE ESTA: un asset por nivel en Data/Unir (Nivel1, Nivel2...).
// PARA CAMBIAR: dificultad -> "Tinta" (menos tinta = mas dificil) y la lista
//               "Obstaculos". Posiciones -> en fracciones de pantalla (0 a 1).
// PARA UNO NUEVO: Create > Talkin Poku > Nivel Unir y sumarlo a la lista
//                 "Niveles" de UnirGame.
// ============================================================================

// Un obstaculo (cerco) del nivel.
[Serializable]
public struct ObstaculoUnir
{
    // En fracciones de la pantalla (0 a 1), igual que los personajes.
    public Vector2 posicion;
    // En unidades de mundo: el sprite cuadrado mide 1 x 1, asi que esto es directamente la escala.
    public Vector2 tamano;
    public float rotacion;
}

// Un nivel es un asset y no codigo: se pueden armar niveles nuevos desde el Inspector sin tocar
// el script.
[CreateAssetMenu(fileName = "NivelUnir", menuName = "Talkin Poku/Nivel Unir")]
public class NivelUnir : ScriptableObject
{
    [Header("Personajes")]
    // Fracciones de la pantalla y no posiciones de mundo: asi el nivel entra en cualquier celular,
    // sea mas angosto o mas ancho.
    [SerializeField] private Vector2 posicionPoku = new Vector2(0.25f, 0.2f);
    [SerializeField] private Vector2 posicionAmigo = new Vector2(0.75f, 0.8f);

    [Header("Dificultad")]
    // [AJUSTE] Tinta del nivel.
    // Largo total que se puede dibujar, en unidades de mundo. Es la perilla principal del nivel.
    [SerializeField] private float tinta = 12f;
    // [AJUSTE] Obstaculos del nivel.
    [SerializeField] private ObstaculoUnir[] obstaculos;

    public Vector2 PosicionPoku => posicionPoku;
    public Vector2 PosicionAmigo => posicionAmigo;
    public float Tinta => tinta;
    public ObstaculoUnir[] Obstaculos => obstaculos;
}
