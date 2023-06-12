using System;
using System.Collections.Generic;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    [RequireComponent(typeof(BuildingMenu))]
    public class MainUI : Singleton<MainUI>
    {
        [NonSerialized] public UnityEvent<IGetCreatureInfo> OnShowCreatureInfo = new(); // send to CreatureInfoUI; invoke at CreatureInGame
        [NonSerialized] public UnityEvent<long> OnRemainStep = new(); // send to StepCounter; invoke at EnvironmentManager
        [NonSerialized] public UnityEvent OnUpdateCurrencies = new(); // send to CurrenciesInfo; invoke at SavingSystemManager
        [NonSerialized] public UnityEvent OnClickIdleButton = new(); // send to PlayerFactionManager; invoke at DontMoveButton & MovingPath
        [NonSerialized] public UnityEvent<FactionType> OnGameOver = new(); // send to GameOverAnnouncer; invoke at PlayerFactionManager
        [NonSerialized] public UnityEvent<List<JIInventoryItem>> OnShowBuildingMenu = new(); // send to BuildingMenu, invoke at InventoryLoader
        [NonSerialized] public UnityEvent OnHideAllMenu = new(); // send to BuildingMenu

        private BuildingMenu _buildingMenu;

        private void Start()
        {
            _buildingMenu = GetComponent<BuildingMenu>();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (PointingChecker.IsPointerOverUIObject() || _buildingMenu.IsInADeal())
                    return;
                
                OnHideAllMenu.Invoke();
            }
        }
    }
}