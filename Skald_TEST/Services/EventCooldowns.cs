
using System.Collections.Generic;

namespace AIInfluence.Services
{
    public static class EventCooldowns
    {
        private static Dictionary<string, int> _cooldowns = new();

        public static bool CanApply(string key, int currentDay, int cooldownDays)
        {
            if (!_cooldowns.ContainsKey(key))
            {
                _cooldowns[key] = currentDay;
                return true;
            }
            return currentDay - _cooldowns[key] >= cooldownDays;
        }

        public static Dictionary<string, int> Data => _cooldowns;
    }
}
