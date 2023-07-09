using System;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class BattleMainUI : MainUI
    {
        protected override void Start()
        {
            _creatureMenu = GetComponent<CreatureMenu>();
            
            OnEnableInteract.AddListener(EnableInteractable);
            OnGameOver.AddListener(DisableInteractable);
        }

        private void EnableInteractable()
        {
            IsInteractable = true;
        }

        private void DisableInteractable()
        {
            IsInteractable = false;
        }

        protected override void Update() { }
    }
}