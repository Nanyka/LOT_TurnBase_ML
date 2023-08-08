using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public class UnlockComp : MonoBehaviour
    {
        [SerializeField] private string[] _inventoryId;
        [SerializeField] private bool _isUnlockWhenKilled;

        #region UNLOCK WHEN BE KILLED

        private void OnEnable()
        {
            if (_isUnlockWhenKilled)
                GetComponent<Entity>().OnUnitDie.AddListener(Unlock);
        }

        private async void Unlock(Entity killBy)
        {
            foreach (var unlockedInventory in _inventoryId)
                SavingSystemManager.Instance.GrantInventory(unlockedInventory);
            await SavingSystemManager.Instance.RefreshEconomy();

            // Show Unlock new character panel in Monster Mode
            if (GameFlowManager.Instance.IsEcoMode == false)
                StartCoroutine(WaitToEndGame());
        }

        private IEnumerator WaitToEndGame()
        {
            yield return new WaitForSeconds(1.5f);
            GameFlowManager.Instance.OnGameOver.Invoke();
        }

        #endregion

        #region UNLOCK WHEN BE CALLED

        public void OnAskForUnlock()
        {
            CheckToUnlock();
        }

        private async void CheckToUnlock()
        {
            if (!GameFlowManager.Instance.IsEcoMode) return;
            
            foreach (var unlockedInventory in _inventoryId)
                SavingSystemManager.Instance.GrantInventory(unlockedInventory);
            await SavingSystemManager.Instance.RefreshEconomy();
        }

        #endregion
    }
}