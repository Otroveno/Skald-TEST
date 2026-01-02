using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using Skald.Core;

namespace Skald
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            SkaldBootstrap.Initialize();
        }

        // Ajoutez "public" ici
        public override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);

            // Utilisez le nom complet
            if (game.GameType is TaleWorlds.CampaignSystem.Campaign)
            {
                SkaldBootstrap.OnCampaignLoaded();
            }
        }
    }
}