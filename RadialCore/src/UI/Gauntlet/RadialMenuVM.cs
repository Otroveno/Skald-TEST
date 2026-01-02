using System;
using System.Collections.Generic;
using System.Linq;
using RadialCore.Contracts.Models;
using RadialCore.Core.Diagnostics;

namespace RadialCore.UI.Gauntlet
{
    /// <summary>
    /// ViewModel pour le menu radial.
    /// Gère la logique UI du menu radial (entries, selection, hover).
    /// TODO: Hériter de TaleWorlds ViewModel base class quand disponible.
    /// </summary>
    public class RadialMenuVM
    {
        private readonly MenuContext _context;
        private readonly List<RadialMenuEntry> _entries;
        private readonly Action<RadialMenuEntry> _onEntrySelected;
        private readonly Action<RadialMenuEntry> _onEntryHovered;

        // Observable properties (TODO: implement INotifyPropertyChanged)
        public List<RadialMenuEntryVM> Entries { get; private set; }
        public RadialMenuEntryVM? SelectedEntry { get; private set; }
        public bool IsOpen { get; private set; }

        public RadialMenuVM(
            MenuContext context,
            List<RadialMenuEntry> entries,
            Action<RadialMenuEntry> onEntrySelected,
            Action<RadialMenuEntry> onEntryHovered)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _entries = entries ?? throw new ArgumentNullException(nameof(entries));
            _onEntrySelected = onEntrySelected ?? throw new ArgumentNullException(nameof(onEntrySelected));
            _onEntryHovered = onEntryHovered ?? throw new ArgumentNullException(nameof(onEntryHovered));

            Entries = new List<RadialMenuEntryVM>();
            IsOpen = true;

            Initialize();
        }

        private void Initialize()
        {
            try
            {
                Logger.Debug("RadialMenuVM", $"Initializing VM with {_entries.Count} entries");

                // Convert entries to ViewModels
                foreach (var entry in _entries.Where(e => e.IsVisible))
                {
                    var entryVM = new RadialMenuEntryVM(entry, OnEntryClick, OnEntryHover);
                    Entries.Add(entryVM);
                }

                Logger.Info("RadialMenuVM", $"RadialMenuVM initialized with {Entries.Count} visible entries");
            }
            catch (Exception ex)
            {
                Logger.Error("RadialMenuVM", "Failed to initialize RadialMenuVM", ex);
            }
        }

        private void OnEntryClick(RadialMenuEntryVM entryVM)
        {
            try
            {
                if (!entryVM.IsEnabled)
                {
                    Logger.Debug("RadialMenuVM", $"Entry {entryVM.EntryId} is disabled, ignoring click");
                    return;
                }

                SelectedEntry = entryVM;
                _onEntrySelected(entryVM.Entry);
            }
            catch (Exception ex)
            {
                Logger.Error("RadialMenuVM", "Error in OnEntryClick", ex);
            }
        }

        private void OnEntryHover(RadialMenuEntryVM entryVM)
        {
            try
            {
                _onEntryHovered(entryVM.Entry);
            }
            catch (Exception ex)
            {
                Logger.Error("RadialMenuVM", "Error in OnEntryHover", ex);
            }
        }

        public void Close()
        {
            IsOpen = false;
        }
    }

    /// <summary>
    /// ViewModel pour une entrée de menu radial.
    /// </summary>
    public class RadialMenuEntryVM
    {
        public RadialMenuEntry Entry { get; }
        private readonly Action<RadialMenuEntryVM> _onClick;
        private readonly Action<RadialMenuEntryVM> _onHover;

        public string EntryId => Entry.EntryId;
        public string Label => Entry.Label;
        public string Description => Entry.Description;
        public string IconId => Entry.IconId;
        public bool IsEnabled => Entry.IsEnabled;
        public bool IsVisible => Entry.IsVisible;

        public RadialMenuEntryVM(
            RadialMenuEntry entry,
            Action<RadialMenuEntryVM> onClick,
            Action<RadialMenuEntryVM> onHover)
        {
            Entry = entry ?? throw new ArgumentNullException(nameof(entry));
            _onClick = onClick ?? throw new ArgumentNullException(nameof(onClick));
            _onHover = onHover ?? throw new ArgumentNullException(nameof(onHover));
        }

        public void ExecuteClick()
        {
            _onClick(this);
        }

        public void ExecuteHover()
        {
            _onHover(this);
        }
    }
}
