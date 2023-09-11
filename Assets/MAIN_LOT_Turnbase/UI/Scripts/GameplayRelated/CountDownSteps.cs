using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class CountDownSteps : MonoBehaviour
    {
        [SerializeField] private GameObject _clock;
        [SerializeField] private TextMeshProUGUI _clockText;
        
        private int _remainSteps;
        
        private void Start()
        {
            GameFlowManager.Instance.OnKickOffEnv.AddListener(Init);
        }

        private void Init()
        {
            GameFlowManager.Instance.GetEnvManager().OnChangeFaction.AddListener(StartCountDown);
            
            if (GameFlowManager.Instance.GameMode == GameMode.BOSS)
                _remainSteps = GameFlowManager.Instance.GetQuest().maxMovingTurn;
            UpdateRemainStepUI();
            _clock.SetActive(true);
        }

        private void StartCountDown()
        {
            if (GameFlowManager.Instance.GetEnvManager().GetCurrFaction() == FactionType.Player)
                CountDown();
        }

        private void CountDown()
        {
            _remainSteps--;
            UpdateRemainStepUI();

            if (_remainSteps <= 0)
                GameFlowManager.Instance.OnGameOver.Invoke(0);
        }

        private void UpdateRemainStepUI()
        {
            _clockText.text = _remainSteps.ToString();
        }

        public int GetRemainSteps()
        {
            return _remainSteps;
        }
    }
}