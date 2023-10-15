using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "CollectableStats", menuName = "JumpeeIsland/CollectableStats", order = 7)]
    public class CollectableStats : ScriptableObject
    {
        public bool IsLongLasting;
        [ShowIf("@IsLongLasting == false")] public int MaxTurnToDestroy;
        [Tooltip("If it's true, the collectable item can turn to reward after being destroyed")] 
        public bool IsSelfCollect;
        public CollectableType CollectableType;
        public string SkinAddress;
        
        [Header("Currency rewards")]
        [VerticalGroup("RewardPart", VisibleIf = "@CollectableType == JumpeeIsland.CollectableType.REWARD")]
        [VerticalGroup("RewardPart/Row1")]
        public List<CommandName> Commands;

        [Header("Entity rewards")] 
        [VerticalGroup("RewardPart/Row2")]
        public EntityType SpawnedEntityType;
        [ShowIf("@SpawnedEntityType != EntityType.NONE")] public string EntityName;
        
        [ShowIf("@CollectableType == JumpeeIsland.CollectableType.TRAP")] public int TrapDamage;
        
        [Header("Effect rewards")]
        // [VerticalGroup("RewardPart/Row3")]
        public SkillEffectType _skillEffectType;
        [VerticalGroup("SkillEffect", VisibleIf = "@_skillEffectType != SkillEffectType.None")]
        [VerticalGroup("SkillEffect/Row2")] [SerializeField] private int _duration;
        [VerticalGroup("SkillEffect/Row3")] [SerializeField] private int _magnitude;
        private SkillEffect _skillEffect;

        public SkillEffect GetSkillEffect()
        {
            if (_skillEffect == null)
                SetEffectType();
            return _skillEffect;
        }
        
        private void SetEffectType()
        {
            switch (_skillEffectType)
            {
                case SkillEffectType.JumpBoost:
                    _skillEffect = new JumpBoost(_duration,_magnitude);
                    break;
            }
        }
    }
}