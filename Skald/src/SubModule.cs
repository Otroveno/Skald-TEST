using System;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using Skald.Core;
using Skald.CampaignSystem;

namespace Skald
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            SkaldBootstrap.Initialize();
            Console.WriteLine("[Skald] SubModule chargé.");
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarter)
        {
            base.OnGameStart(game, gameStarter);

            if (game.GameType is TaleWorlds.CampaignSystem.Campaign && gameStarter is CampaignGameStarter campaignStarter)
            {
                campaignStarter.AddBehavior(new SkaldCampaignBehavior());
                Console.WriteLine("[Skald] Comportement de campagne ajouté.");
            }
        }
    }
}