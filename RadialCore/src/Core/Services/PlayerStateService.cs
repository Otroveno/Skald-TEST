using System;
using RadialCore.Contracts.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace RadialCore.Core.Services
{
    /// <summary>
    /// Interface pour le service d'état du joueur.
    /// Fournit les infos du joueur (gold, level, health, etc.).
    /// </summary>
    public interface IPlayerStateService
    {
        PlayerInfo? GetPlayerInfo();
    }

    /// <summary>
    /// Implémentation par défaut du service d'état du joueur.
    /// Utilise TaleWorlds.CampaignSystem APIs.
    /// </summary>
    public class PlayerStateService : IPlayerStateService
    {
        public PlayerInfo? GetPlayerInfo()
        {
            try
            {
                // Vérifier que Campaign est chargé
                if (Campaign.Current == null || Hero.MainHero == null)
                    return null;

                var hero = Hero.MainHero;
                
                return new PlayerInfo
                {
                    Name = "Player", // Stub: évite dependency TaleWorlds.Localization
                    Gold = hero.Gold,
                    Level = (int)hero.Level,
                    IsInCombat = false, // TODO: Detect combat state
                    IsOnHorse = false,  // TODO: Detect mount state
                    Health = hero.HitPoints,
                    HealthPercentage = (float)hero.HitPoints / hero.MaxHitPoints
                };
            }
            catch (Exception)
            {
                // Fail gracefully si APIs Bannerlord changent
                return null;
            }
        }
    }
}
