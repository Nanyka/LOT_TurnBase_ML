using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class StringChecker : MonoBehaviour, ITimeMachineChecker
    {
        [SerializeField] private string _targetString;
        
        private string _checkedString;
        
        private void Start()
        {
            TimelineManager.Instance.OnStringCheck.AddListener(SetStringToCheck);
        }

        private void SetStringToCheck(string checkedString)
        {
            _checkedString = checkedString;
        }

        public bool ConditionCheck()
        {
            // Debug.Log($"Checked string: {_checkedString}");
            return _targetString.Equals(_checkedString);
        }
    }
}