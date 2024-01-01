using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class ShowBattleCreatureMenu : MonoBehaviour
    {
        private BattleEnvLoad _battleEnvLoad;

        private void Start()
        {
            _battleEnvLoad = FindObjectOfType<BattleEnvLoad>();
        }

        public void OnOpenCreatureMenu()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            MainUI.Instance.OnShowDropTroopMenu.Invoke(_battleEnvLoad.GetSpawnList());
        }
    }
}