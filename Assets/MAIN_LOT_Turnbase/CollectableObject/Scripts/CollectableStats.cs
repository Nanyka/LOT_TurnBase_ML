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
        public List<CommandName> Commands;

        [Header("Entity rewards")] 
        public EntityType SpawnedEntityType;
        [ShowIf("@SpawnedEntityType != EntityType.NONE")] public string EntityName;
        
        [Header("Effect rewards")]
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