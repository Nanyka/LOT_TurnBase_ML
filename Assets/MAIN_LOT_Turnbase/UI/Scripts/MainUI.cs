using System;
using System.Collections.Generic;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class MainUI : Singleton<MainUI>
    {

        [NonSerialized] public UnityEvent OnEnableInteract = new(); // send to DontMoveButton; invoke here in EcoMode and at CreatureMenu in BattleMode
        [NonSerialized] public UnityEvent<IShowInfo> OnShowInfo = new(); // send to CreatureInfoUI; invoke at PlayerFactionController
        [NonSerialized] public UnityEvent<long> OnRemainStep = new(); // send to StepCounter; invoke at EnvironmentManager
        [NonSerialized] public UnityEvent OnUpdateCurrencies = new(); // send to CurrenciesInfo; invoke at SavingSystemManager
        [NonSerialized] public UnityEvent OnClickIdleButton = new(); // send to PlayerFactionManager; invoke at DontMoveButton & MovingPath
        [NonSerialized] public UnityEvent<FactionType> OnGameOver = new(); // send to GameOverAnnouncer; invoke at PlayerFactionManager
        [NonSerialized] public UnityEvent<List<JIInventoryItem>> OnBuyBuildingMenu = new(); // send to BuyBuildingMenu, invoke at InventoryLoader
        [NonSerialized] public UnityEvent<IConfirmFunction> OnSellBuildingMenu = new(); // send to SellBuildingMenu, invoke at BuildingController
        [NonSerialized] public UnityEvent<List<JIInventoryItem>> OnShowCreatureMenu = new(); // send to CreatureMenu, invoke at InventoryLoader
        [NonSerialized] public UnityEvent OnHideAllMenu = new(); // send to BuildingMenu

        private BuyBuildingMenu _buyBuildingMenu;
        protected CreatureMenu _creatureMenu;

        protected virtual void Start()
        {
            _buyBuildingMenu = GetComponent<BuyBuildingMenu>();
            _creatureMenu = GetComponent<CreatureMenu>();
            
            GameFlowManager.Instance.OnStartGame.AddListener(EnableInteract);
        }

        private void EnableInteract(long arg0)
        {
            OnEnableInteract.Invoke();
        }

        protected virtual void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (PointingChecker.IsPointerOverUIObject() || _buyBuildingMenu.IsInADeal() || _creatureMenu.IsInADeal())
                    return;
                
                OnHideAllMenu.Invoke();
            }
        }
    }
}