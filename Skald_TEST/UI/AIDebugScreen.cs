
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.ScreenSystem;

namespace AIInfluence.UI
{
    public class AIDebugScreen : ScreenBase
    {
        private GauntletLayer _layer;
        private AIDebugVM _vm;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _vm = new AIDebugVM();
            _layer = new GauntletLayer(100);
            _layer.LoadMovie("AIDebug", _vm);
            AddLayer(_layer);
        }

        protected override void OnFinalize()
        {
            RemoveLayer(_layer);
            base.OnFinalize();
        }

        public AIDebugVM ViewModel => _vm;
    }
}
