using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using Skald.Core;

namespace Skald
{
    public class SubModule : TaleWorlds.MountAndBlade.SubModule
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            SkaldBootstrap.Initialize();
        }

        protected override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);
            if (game.GameType is Campaign)
                SkaldBootstrap.OnCampaignLoaded();
        }
    }
}