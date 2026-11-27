// ============================================================================
// XpSource.cs
// QUE HACE: las actividades que dan experiencia. Se usa como
//           progreso.Ganar(XpSource.Comer).
// PARA CAMBIAR: cuanta XP da cada una -> lista "Premios" en
//               Data/Progression/ProgressionConfig.asset.
//               Para una fuente nueva: agregarla aca, sumarla a Premios en el
//               asset y llamar progreso.Ganar(XpSource.LaNueva) donde pase.
// ============================================================================

// De donde sale la experiencia. Cada fuente tiene su premio y su pausa en ProgressionConfig.
public enum XpSource
{
    Comer,      // PetDropTarget.Comer
    Limpiar,    // PetStains, al borrar una mancha
    Acariciar,  // PetInteraction, al empezar una caricia
    Pelota,     // Ball, cuando le pega a Poku
    Dormir,     // Progression, al despertarse con la energia llena
    Minijuego   // MinigameReward, al volver de un minijuego
}
