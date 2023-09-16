using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class AutomationButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _stateText;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Color _onColor;
        [SerializeField] private Color _offColor;

        private bool _isAutomation = true;

        private void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(HideButton);
            
            _stateText.text = "ON";
            _buttonImage.color = _onColor;
        }

        private void HideButton()
        {
            GameFlowManager.Instance.OnKickOffEnv.RemoveListener(HideButton);
            gameObject.SetActive(false);
        }

        public void OnClickAutomation()
        {
            _isAutomation = !_isAutomation;
            GameFlowManager.Instance.OnChangAutomationMode.Invoke(_isAutomation);
            _stateText.text = _isAutomation?"ON": "OFF";
            _buttonImage.color = _isAutomation ? _onColor : _offColor;
        }
    }
}