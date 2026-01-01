
using TaleWorlds.Library;
using System.Collections.ObjectModel;

namespace AIInfluence.UI
{
    public class AIDebugVM : ViewModel
    {
        public ObservableCollection<string> Events { get; } = new();

        private bool _aiEnabled = true;
        public bool AIEnabled
        {
            get => _aiEnabled;
            set
            {
                if (value != _aiEnabled)
                {
                    _aiEnabled = value;
                    OnPropertyChanged(nameof(AIEnabled));
                }
            }
        }

        public void AddEvent(string evt)
        {
            Events.Insert(0, evt);
            if (Events.Count > 20)
                Events.RemoveAt(Events.Count - 1);
        }
    }
}
