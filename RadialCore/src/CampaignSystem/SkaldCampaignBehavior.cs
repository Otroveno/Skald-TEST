using Skald.Core;
using Skald.Core.Interfaces;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace Skald.CampaignSystem
{
    public class SkaldCampaignBehavior : CampaignBehaviorBase
    {
        private readonly ISkaldAIAdapter _aiAdapter;

        public SkaldCampaignBehavior()
        {
            _aiAdapter = SkaldBootstrap.ActiveAI;
        }

        public override void RegisterEvents()
        {
            // Liez vos méthodes aux événements de la campagne
            // Ex: CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // Pour sauvegarder/charger des données
        }

        // Exemple de méthode appelée par un événement
        private void OnDailyTick()
        {
            // Utilisez votre adaptateur ici
            // _aiAdapter.DoSomething();
        }
    }
}