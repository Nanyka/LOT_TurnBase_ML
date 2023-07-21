using UnityEngine;

namespace JumpeeIsland
{
    public class MonsterResultPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _losePanel;
        
        private void Start()
        {
            MainUI.Instance.OnGameOver.AddListener(ShowGameOverPanel);
        }

        private void ShowGameOverPanel()
        {
            var creatures = SavingSystemManager.Instance.GetEnvironmentData().EnemyData;

            int totalAmount = 0;
            foreach (var creature in creatures)
                if (creature.CreatureType == CreatureType.BOSS)
                    totalAmount++;
            
            if (totalAmount > 0)
                _losePanel.SetActive(true);
            else
                _winPanel.SetActive(true);
        }
    }
}