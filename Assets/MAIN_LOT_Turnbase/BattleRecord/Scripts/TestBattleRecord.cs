using UnityEngine;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "BattleRecord", menuName = "JumpeeIsland/BattleRecord", order = 9)]
    public class TestBattleRecord : ScriptableObject
    {
        public BattleRecord BattleRecord;
    }
}