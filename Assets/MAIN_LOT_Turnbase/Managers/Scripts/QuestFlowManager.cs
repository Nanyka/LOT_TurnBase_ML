using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class QuestFlowManager : GameFlowManager
    {
        [SerializeField] private Quest _quest;

        protected override void ConfirmGameStarted()
        {
            base.ConfirmGameStarted();
            if (_quest.targetPos.x.Equals(float.NegativeInfinity))
                return;

            var target = _environmentManager.GetObjectByPosition(_quest.targetPos, FactionType.Enemy);
            if (_quest.targetType == EntityType.RESOURCE)
                target = _environmentManager.GetObjectByPosition(_quest.targetPos, FactionType.Neutral);
            
            if (target != null)
                target.AddComponent<EndGameComp>();
        }

        public override Quest GetQuest()
        {
            return _quest;
        }
    }
}