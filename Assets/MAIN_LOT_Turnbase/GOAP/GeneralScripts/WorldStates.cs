using System.Collections.Generic;
using UnityEngine;

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

        /// <summary>
        ///   <para>Add 1 State into local belief. If the State existed, add nothing</para>
        /// </summary>
        public void AddState(string key)
        {
            if (HasState(key))
                return;

            _states.Add(key, 1);
        }

        public void ModifyState(string key, int value)
        {
            if (HasState(key))
            {
                _states[key] += value;
                if (_states[key] <= 0)
                    RemoveState(key);
            }
            else if (value > 0)
                _states.Add(key, value);
        }

        public void RemoveState(string key)
        {
            if (HasState(key))
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