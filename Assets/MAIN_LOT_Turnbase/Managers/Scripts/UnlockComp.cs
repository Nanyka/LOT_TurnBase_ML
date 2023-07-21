using System;
using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class UnlockComp : MonoBehaviour
    {
        [SerializeField] private string _inventoryId;
        
        private void Start()
        {
            GetComponent<Entity>().OnUnitDie.AddListener(Unlock);
        }

        private void Unlock(Entity killBy)
        {
            SavingSystemManager.Instance.GrantInventory(_inventoryId);
            
            // Show Unlock new character panel in Monster Mode
            if (GameFlowManager.Instance.IsEcoMode == false)
                StartCoroutine(WaitToEndGame());
        }

        private IEnumerator WaitToEndGame()
        {
            yield return new WaitForSeconds(3f);
            MainUI.Instance.OnGameOver.Invoke();
        }
    }
}