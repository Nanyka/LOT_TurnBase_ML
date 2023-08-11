using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class UnlockComp : MonoBehaviour
    {
        [SerializeField] private string _inventoryId;
        [SerializeField] private bool _isUnlockCreature;

        #region UNLOCK CREATURE

        private void OnEnable()
        {
            if (_isUnlockCreature)
                GetComponent<Entity>().OnUnitDie.AddListener(Unlock);
        }

        private async void Unlock(Entity killBy)
        {
            SavingSystemManager.Instance.GrantInventory(_inventoryId,0);
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

        #region UNLOCK BUILDING

        public void OnAskForUnlock()
        {
            CheckToUnlock();
        }

        private async void CheckToUnlock()
        {
            if (!GameFlowManager.Instance.IsEcoMode) return;

            SavingSystemManager.Instance.GrantInventory(_inventoryId);
            await SavingSystemManager.Instance.RefreshEconomy();
        }

        #endregion
    }
}