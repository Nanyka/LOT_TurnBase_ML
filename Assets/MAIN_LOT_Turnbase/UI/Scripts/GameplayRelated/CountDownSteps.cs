using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class CountDownSteps : MonoBehaviour
    {
        [SerializeField] private int _maxSteps;
        [SerializeField] private GameObject _clock;
        [SerializeField] private TextMeshProUGUI _clockText;
        
        private void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(Init);
        }

        private void Init()
        {
            GameFlowManager.Instance.GetEnvManager().OnChangeFaction.AddListener(StartCountDown);
            CountDown();
            _clock.SetActive(true);
        }

        private void StartCountDown()
        {
            if (GameFlowManager.Instance.GetEnvManager().GetCurrFaction() == FactionType.Player)
                CountDown();

        }

        private void CountDown()
        {
            _maxSteps--;
            UpdateRemainStepUI();

            if (_maxSteps <= 0)
                GameFlowManager.Instance.OnGameOver.Invoke();
        }

        private void UpdateRemainStepUI()
        {
            _clockText.text = _maxSteps.ToString();
        }
    }
}