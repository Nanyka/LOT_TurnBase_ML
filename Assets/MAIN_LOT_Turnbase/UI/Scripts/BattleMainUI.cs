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
            
            OnEnableInteract.AddListener(SetInteractable);
        }

        private void SetInteractable()
        {
            IsInteractable = true;
        }

        protected override void Update() { }
    }
}