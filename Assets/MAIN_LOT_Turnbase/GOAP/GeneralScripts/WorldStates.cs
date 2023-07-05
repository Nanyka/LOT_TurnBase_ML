using System.Collections.Generic;

namespace GOAP
{
    public class WorldStates
    {
        private Dictionary<string, int> _states;

        public WorldStates()
        {
            _states = new Dictionary<string, int>();
        }

        public bool HasState(string key)
        {
            return _states.ContainsKey(key);
        }

        public void AddState(string key, int value)
        {
            _states.Add(key, value);
        }

        public void ModifyState(string key, int value)
        {
            if (HasState(key))
            {
                _states[key] += value;
                if (_states[key] <= 0)
                    RemoveState(key);
            }
            else
                _states.Add(key, value);
        }

        public void RemoveState(string key)
        {
            if (_states.ContainsKey(key))
                _states.Remove(key);
        }

        public void ClearStates()
        {
            _states.Clear();
        }

        public Dictionary<string, int> GetStates()
        {
            return _states;
        }
    }
}