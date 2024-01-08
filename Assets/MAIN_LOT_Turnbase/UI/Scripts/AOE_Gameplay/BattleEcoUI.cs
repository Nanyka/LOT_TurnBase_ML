using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class BattleEcoUI : MainUI
    {
        protected override void Start()
        {
            // _mainCamera = Camera.main;
            GameFlowManager.Instance.OnDataLoaded.AddListener(EnableInteract);
        }
    }
}
