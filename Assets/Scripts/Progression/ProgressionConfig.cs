using System;
using System.Collections.Generic;
using UnityEngine;

/// Numeros de la progresion: cuanta experiencia pide cada nivel, cuanta da cada actividad y en que
/// nivel empieza cada evolucion. Van en un asset para poder balancear sin tocar codigo.
[CreateAssetMenu(menuName = "TalkinPoku/Progression Config", fileName = "ProgressionConfig")]
public class ProgressionConfig : ScriptableObject
{
    [Serializable]
    public struct Premio
    {
        public XpSource fuente;
        public int xp;
        // Sin pausa, soltar la comida una y otra vez sobre Poku seria la forma mas rapida de subir.
        public float segundosEntrePremios;
    }

    [Serializable]
    public struct Etapa
    {
        public string nombre;
        public int nivelDesde;
        public PetAppearance apariencia;
    }

    [Header("Niveles")]
    [SerializeField] private int nivelMaximo = 50;
    // Lo que pide pasar del nivel 1 al 2. Cada nivel pide el incremento mas que el anterior: con 20 y 2,
    // llegar a Pokucha (nivel 20) son unos 720 puntos, un par de dias jugando normal.
    [SerializeField] private int xpPrimerNivel = 20;
    [SerializeField] private int incrementoPorNivel = 2;

    [Header("Premios")]
    [SerializeField] private Premio[] premios;

    [Header("Etapas")]
    // De menor a mayor nivelDesde, y la primera desde el nivel 1. Mega-Pokuchan no va aca: no se llega
    // subiendo de nivel sino activandola.
    [SerializeField] private Etapa[] etapas;

    [Header("Mega Evolucion")]
    [SerializeField] private int nivelMega = 50;
    // Se carga con la misma experiencia que dan las actividades: con 200, unas 7 partidas de
    // minijuego o un rato largo de cuidarlo. No hay forma de cargarla sin jugar.
    [SerializeField] private int energiaMega = 200;
    [SerializeField] private float minutosMega = 30f;
    [SerializeField] private float multiplicadorMonedasMega = 1.5f;
    [SerializeField] private PetAppearance aparienciaMega;

    public int NivelMaximo => nivelMaximo;
    public IReadOnlyList<Etapa> Etapas => etapas;
    public int NivelMega => nivelMega;
    public int EnergiaMega => energiaMega;
    public float MinutosMega => minutosMega;
    public float MultiplicadorMonedasMega => multiplicadorMonedasMega;
    public PetAppearance AparienciaMega => aparienciaMega;

    public int XpParaSubir(int nivel)
    {
        return xpPrimerNivel + incrementoPorNivel * (nivel - 1);
    }

    public bool BuscarPremio(XpSource fuente, out Premio premio)
    {
        foreach (Premio p in premios)
        {
            if (p.fuente != fuente) continue;
            premio = p;
            return true;
        }

        premio = default;
        return false;
    }

    public int EtapaDe(int nivel)
    {
        int etapa = 0;
        for (int i = 0; i < etapas.Length; i++)
        {
            if (nivel >= etapas[i].nivelDesde) etapa = i;
        }
        return etapa;
    }
}
