using UnityEngine;

namespace JumpeeIsland
{
    public class BattleMainUI : MainUI
    {
        protected override void Start()
        {
            _creatureMenu = GetComponent<CreatureMenu>();
        }
        
        protected override void Update() { }
    }
}