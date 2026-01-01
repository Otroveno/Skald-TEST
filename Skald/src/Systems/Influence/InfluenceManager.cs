using System.Collections.Generic;

namespace Skald.Systems.Influence
{
    public class InfluenceManager
    {
        private Dictionary<string, float> _values = new();

        public void Add(string key, float value)
        {
            if (!_values.ContainsKey(key)) _values[key] = 0;
            _values[key] += value;
        }

        public float Get(string key)
        {
            return _values.TryGetValue(key, out var v) ? v : 0f;
        }
    }
}