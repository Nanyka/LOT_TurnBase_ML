using System;
using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class CountDownClock : MonoBehaviour
    {
        [SerializeField] private GameObject _clock;
        [SerializeField] private TextMeshProUGUI _clockText;

        private static readonly int _battleDuration = 180;
        private static int _countDown;

        private void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(StartCountDown);
        }

        private void StartCountDown()
        {
            _clock.SetActive(true);
            UpdateClockUI();
            // Start count down clock
            _countDown = _battleDuration;
            InvokeRepeating(nameof(UpdateClock), 1f, 1f);
        }

        private void UpdateClock()
        {
            _countDown--;
            UpdateClockUI();

            if (_countDown <= 0)
            {
                CancelInvoke();
                GameFlowManager.Instance.OnGameOver.Invoke(0);
            }
        }

        private void UpdateClockUI()
        {
            _clockText.text = _countDown.ToString();
        }

        public static int GetBattleTime()
        {
            return _battleDuration - _countDown;
        }
    }
}