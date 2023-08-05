using System;
using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class UnlockComp : MonoBehaviour
    {
        [SerializeField] private string _inventoryId;
        
        private void OnEnable()
        {
            GetComponent<Entity>().OnUnitDie.AddListener(Unlock);
        }

        private async void Unlock(Entity killBy)
        {
            SavingSystemManager.Instance.GrantInventory(_inventoryId);
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
    }
}