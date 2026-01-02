using System;
using RadialCore.Core;
using RadialCore.Core.Diagnostics;
using RadialCore.Core.Events;

namespace RadialCore.Input
{
    public class InputManager
    {
        private readonly ContextHub _contextHub;
        
        private InputKey _hotkeyPrimary = InputKey.V;
        private InputKey _hotkeyModifier = InputKey.Invalid;
        private bool _isHoldMode = false;
        
        private bool _isMenuOpen = false;
        private bool _wasKeyDownLastFrame = false;
        private float _holdDuration = 0f;
        private const float MIN_HOLD_DURATION = 0.1f;

        public bool IsMenuOpen => _isMenuOpen;

        public InputManager(ContextHub contextHub)
        {
            _contextHub = contextHub ?? throw new ArgumentNullException(nameof(contextHub));
            Logger.Info("InputManager", "InputManager initialized");
            Logger.Info("InputManager", $"Hotkey: {_hotkeyPrimary} (Mode: {(_isHoldMode ? "Hold" : "Toggle")})");
        }

        public void OnTick(float deltaTime)
        {
            try
            {
                bool isKeyDown = IsHotkeyPressed();

                if (_isHoldMode)
                {
                    HandleHoldMode(isKeyDown, deltaTime);
                }
                else
                {
                    HandleToggleMode(isKeyDown);
                }

                _wasKeyDownLastFrame = isKeyDown;
            }
            catch (Exception ex)
            {
                Logger.Error("InputManager", "Exception in OnTick", ex);
            }
        }

        private bool IsHotkeyPressed()
        {
            try
            {
                // TODO: Implement actual input polling
                // For now, return false (stub)
                return false;
            }
            catch
            {
                return false;
            }
        }

        private void HandleHoldMode(bool isKeyDown, float deltaTime)
        {
            if (isKeyDown && !_wasKeyDownLastFrame)
            {
                _holdDuration = 0f;
                OpenMenu();
            }
            else if (isKeyDown && _wasKeyDownLastFrame)
            {
                _holdDuration += deltaTime;
            }
            else if (!isKeyDown && _wasKeyDownLastFrame)
            {
                if (_holdDuration >= MIN_HOLD_DURATION)
                {
                    CloseMenu();
                }
                _holdDuration = 0f;
            }
        }

        private void HandleToggleMode(bool isKeyDown)
        {
            if (isKeyDown && !_wasKeyDownLastFrame)
            {
                if (_isMenuOpen)
                {
                    CloseMenu();
                }
                else
                {
                    OpenMenu();
                }
            }
        }

        private void OpenMenu()
        {
            if (_isMenuOpen) return;

            Logger.Info("InputManager", "Opening radial menu");
            _isMenuOpen = true;

            _contextHub.ForceRefresh();
            var context = _contextHub.GetCurrentContext();

            EventBus.Instance.Publish(new RadialMenuOpenedEvent
            {
                Timestamp = GetCurrentTime(),
                Context = context
            });

            Logger.Info("InputManager", "Radial menu opened (UI not yet implemented)");
        }

        private void CloseMenu()
        {
            if (!_isMenuOpen) return;

            Logger.Info("InputManager", "Closing radial menu");
            _isMenuOpen = false;

            EventBus.Instance.Publish(new RadialMenuClosedEvent
            {
                Timestamp = GetCurrentTime(),
                WasActionExecuted = false
            });

            Logger.Info("InputManager", "Radial menu closed");
        }

        private float GetCurrentTime()
        {
            return (float)(DateTime.Now - DateTime.Today).TotalSeconds;
        }

        public void ForceCloseMenu()
        {
            CloseMenu();
        }

        public void SetHotkey(InputKey primary, InputKey modifier = InputKey.Invalid)
        {
            _hotkeyPrimary = primary;
            _hotkeyModifier = modifier;
            Logger.Info("InputManager", $"Hotkey changed to: {_hotkeyPrimary}" + 
                (modifier != InputKey.Invalid ? $" + {modifier}" : ""));
        }

        public void SetMode(bool isHoldMode)
        {
            _isHoldMode = isHoldMode;
            Logger.Info("InputManager", $"Mode changed to: {(_isHoldMode ? "Hold" : "Toggle")}");
        }

        public (InputKey primary, InputKey modifier, bool isHoldMode) GetConfiguration()
        {
            return (_hotkeyPrimary, _hotkeyModifier, _isHoldMode);
        }
    }
}
