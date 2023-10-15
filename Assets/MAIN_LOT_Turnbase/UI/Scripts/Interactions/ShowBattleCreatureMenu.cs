using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class ShowBattleCreatureMenu : MonoBehaviour
    {
        private BattleEnvLoader _battleEnvLoader;

        private void Start()
        {
            _battleEnvLoader = FindObjectOfType<BattleEnvLoader>();
        }

        public void OnOpenCreatureMenu()
        {
            MainUI.Instance.OnHideAllMenu.Invoke();
            MainUI.Instance.OnShowDropTroopMenu.Invoke(_battleEnvLoader.GetSpawnList());
        }
    }
}