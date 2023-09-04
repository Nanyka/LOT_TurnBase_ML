using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class CreatureUnlockComp : MonoBehaviour
    {
        [SerializeField] private string _inventoryId;

        #region UNLOCK CREATURE

        private void OnEnable()
        {
            GetComponent<Entity>().OnUnitDie.AddListener(Unlock);
        }

        private async void Unlock(Entity killBy)
        {
            SavingSystemManager.Instance.GrantInventory(_inventoryId, 0);
            await SavingSystemManager.Instance.RefreshEconomy();

            // Show Unlock new character panel in Monster Mode
            if (GameFlowManager.Instance.GameMode == GameMode.BOSS)
                StartCoroutine(WaitToEndGame());
        }

        private IEnumerator WaitToEndGame()
        {
            yield return new WaitForSeconds(2f);
            GameFlowManager.Instance.OnKilledBoss.Invoke();
        }

        #endregion
    }
}