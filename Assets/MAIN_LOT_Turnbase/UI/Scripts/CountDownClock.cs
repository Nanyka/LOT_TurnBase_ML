using System;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class CountDownClock : MonoBehaviour
    {
        [SerializeField] private int _gameDuration;
        [SerializeField] private TextMeshProUGUI _clockText;

        private void Start()
        {
            MainUI.Instance.OnEnableInteract.AddListener(StartCountDown);
        }

        private void StartCountDown()
        {
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
                MainUI.Instance.OnGameOver.Invoke();
            }
        }

        private void UpdateClockUI()
        {
            _clockText.text = _gameDuration.ToString();
        }
    }
}