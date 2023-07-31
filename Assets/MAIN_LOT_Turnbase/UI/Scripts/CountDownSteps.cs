using TMPro;
using UnityEngine;

namespace JumpeeIsland
{
    public class CountDownSteps : MonoBehaviour
    {
        [SerializeField] private int _maxSteps;
        [SerializeField] private TextMeshProUGUI _clockText;
        
        private void Start()
        {
            GameFlowManager.Instance.OnStartGame.AddListener(Init);
        }

        private void Init(long arg0)
        {
            GameFlowManager.Instance.GetEnvManager().OnChangeFaction.AddListener(StartCountDown);
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
                MainUI.Instance.OnGameOver.Invoke();
        }

        private void UpdateRemainStepUI()
        {
            _clockText.text = _maxSteps.ToString();
        }
    }
}