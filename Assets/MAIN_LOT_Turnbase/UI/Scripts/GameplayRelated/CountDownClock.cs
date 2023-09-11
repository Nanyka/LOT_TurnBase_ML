using System;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class CountDownClock : MonoBehaviour
    {
        [SerializeField] private GameObject _clock;
        [SerializeField] private int _gameDuration;
        [SerializeField] private TextMeshProUGUI _clockText;

        private void Start()
        {
            MainUI.Instance.OnEnableInteract.AddListener(StartCountDown);
        }

        private void StartCountDown()
        {
            _clock.SetActive(true);
            UpdateClockUI();
            // Start count down clock
            InvokeRepeating(nameof(UpdateClock), 1f, 1f);

        }

        private void UpdateClock()
        {
            _gameDuration--;
            UpdateClockUI();

            if (_gameDuration <= 0)
            {
                CancelInvoke();
                GameFlowManager.Instance.OnGameOver.Invoke(0);
            }
        }

        private void UpdateClockUI()
        {
            _clockText.text = _gameDuration.ToString();
        }
    }
}