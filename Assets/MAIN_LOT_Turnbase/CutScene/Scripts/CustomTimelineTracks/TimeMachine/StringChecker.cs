using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class StringChecker : MonoBehaviour, ITimeMachineChecker
    {
        [SerializeField] private string _targetString;
        [SerializeField] private ButtonRequire _buttonRequire;
        
        private string _checkedString;
        
        private void Start()
        {
            TimelineManager.Instance.OnStringCheck.AddListener(SetStringToCheck);
        }

        private void SetStringToCheck(string checkedString, ButtonRequire buttonRequire)
        {
            if (_buttonRequire != ButtonRequire.NONE && buttonRequire != _buttonRequire)
                return;
            
            _checkedString = checkedString;
        }

        public bool ConditionCheck()
        {
            // Debug.Log($"Checked string: {_checkedString}");
            return _targetString.Equals(_checkedString);
        }
    }
}