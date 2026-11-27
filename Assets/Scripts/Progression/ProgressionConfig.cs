using System;
using System.Collections.Generic;
using UnityEngine;

// ============================================================================
// ProgressionConfig.cs
// QUE HACE: guarda los numeros de la progresion: cuanta XP pide cada nivel,
//           cuanta XP da cada actividad, en que nivel evoluciona y los datos de
//           la Mega Evolucion. Es un asset para balancear sin tocar codigo.
// DONDE ESTA: Data/Progression/ProgressionConfig.asset (se edita en el Inspector).
// OJO: los numeros de abajo son solo los valores por defecto. El valor real es
//      el que figura en el Inspector de ProgressionConfig.asset.
// QUE SE PUEDE CAMBIAR (en el asset):
//   - nivel maximo -> Nivel Maximo
//   - XP para subir de nivel -> Xp Primer Nivel + Incremento Por Nivel
//     (formula: xpPrimerNivel + incrementoPorNivel * (nivel - 1))
//   - XP de cada actividad y la pausa entre premios -> lista Premios
//   - nivel en que evoluciona y su apariencia -> lista Etapas (Poku, Pokucha, Pokuchan)
//   - Mega: nivel para desbloquearla, energia necesaria, duracion en minutos,
//     multiplicador de monedas y apariencia -> seccion Mega Evolucion
// ============================================================================

// Numeros de la progresion: cuanta experiencia pide cada nivel, cuanta da cada actividad y en que
// nivel empieza cada evolucion. Van en un asset para poder balancear sin tocar codigo.
[CreateAssetMenu(menuName = "TalkinPoku/Progression Config", fileName = "ProgressionConfig")]
public class ProgressionConfig : ScriptableObject
{
    // Cuanta XP da una actividad y cada cuanto puede volver a darla.
    [Serializable]
    public struct Premio
    {
        public XpSource fuente;
        public int xp;
        // Sin pausa, soltar la comida una y otra vez sobre Poku seria la forma mas rapida de subir.
        public float segundosEntrePremios;
    }

    // Una evolucion: nombre, desde que nivel y con que apariencia.
    [Serializable]
    public struct Etapa
    {
        public string nombre;
        public int nivelDesde;
        public PetAppearance apariencia;
    }

    [Header("Niveles")]
    // [AJUSTE] Nivel maximo.
    [SerializeField] private int nivelMaximo = 50;
    // [AJUSTE] XP del primer nivel y cuanto mas pide cada nivel siguiente.
    // Lo que pide pasar del nivel 1 al 2. Cada nivel pide el incremento mas que el anterior: con 20 y 2,
    // llegar a Pokucha (nivel 20) son unos 720 puntos, un par de dias jugando normal.
    [SerializeField] private int xpPrimerNivel = 20;
    [SerializeField] private int incrementoPorNivel = 2;

    [Header("Premios")]
    // [AJUSTE] XP por actividad (Comer, Limpiar, Acariciar, Pelota, Dormir, Minijuego).
    [SerializeField] private Premio[] premios;

    [Header("Etapas")]
    // [AJUSTE] Evoluciones y su nivel.
    // De menor a mayor nivelDesde, y la primera desde el nivel 1. Mega-Pokuchan no va aca: no se llega
    // subiendo de nivel sino activandola.
    [SerializeField] private Etapa[] etapas;

    [Header("Mega Evolucion")]
    // [AJUSTE] Nivel desde el que se puede cargar la Mega.
    [SerializeField] private int nivelMega = 50;
    // [AJUSTE] Energia (XP) necesaria para activarla.
    // Se carga con la misma experiencia que dan las actividades: con 200, unas 7 partidas de
    // minijuego o un rato largo de cuidarlo. No hay forma de cargarla sin jugar.
    [SerializeField] private int energiaMega = 200;
    // [AJUSTE] Cuanto dura (minutos reales).
    [SerializeField] private float minutosMega = 30f;
    // [AJUSTE] Multiplicador de monedas de los minijuegos con la Mega activa (1.5 = 50% mas).
    [SerializeField] private float multiplicadorMonedasMega = 1.5f;
    [SerializeField] private PetAppearance aparienciaMega;

    public int NivelMaximo => nivelMaximo;
    public IReadOnlyList<Etapa> Etapas => etapas;
    public int NivelMega => nivelMega;
    public int EnergiaMega => energiaMega;
    public float MinutosMega => minutosMega;
    public float MultiplicadorMonedasMega => multiplicadorMonedasMega;

    // XP que pide pasar de "nivel" a "nivel + 1".
    public int XpParaSubir(int nivel)
    {
        return xpPrimerNivel + incrementoPorNivel * (nivel - 1);
    }

    // Busca en la lista Premios el de esa fuente. false si no hay premio para esa actividad.
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

    // La de Mega-Pokuchan mientras dure, si no la de la etapa de ese nivel. La usan la mascota de la
    // casa y el avion de Vuelo.
    public PetAppearance AparienciaPara(int nivel)
    {
        if (MegaEvolution.Activa && aparienciaMega != null) return aparienciaMega;
        return etapas[EtapaDe(nivel)].apariencia;
    }

    // Indice de la etapa que corresponde a un nivel (la ultima cuyo nivelDesde ya se alcanzo).
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
