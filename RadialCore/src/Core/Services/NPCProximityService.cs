using System;

namespace RadialCore.Core.Services
{
    /// <summary>
    /// Interface pour le service de proximité NPC.
    /// Détecte les NPCs proches du joueur.
    /// </summary>
    public interface INPCProximityService
    {
        (string id, string name, float distance)? GetNearestNPC(float maxDistance = 10f);
    }

    /// <summary>
    /// Implémentation par défaut du service de proximité NPC.
    /// Utilise Campaign agents pour détecter NPCs proches.
    /// </summary>
    public class NPCProximityService : INPCProximityService
    {
        public (string id, string name, float distance)? GetNearestNPC(float maxDistance = 10f)
        {
            try
            {
                // TODO: Implement NPC proximity detection
                // Requires access to Mission.Current.Agents
                // For now, return null (stub implementation)
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
