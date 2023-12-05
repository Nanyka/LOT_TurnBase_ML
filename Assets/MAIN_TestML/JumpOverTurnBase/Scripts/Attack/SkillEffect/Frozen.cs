using JumpeeIsland;
using UnityEngine;

public class Frozen : SkillEffect
{
    private readonly int _duration;
    private readonly Material _effectMaterial;

    public Frozen(int duration, Material material)
    {
        _duration = duration;
        _effectMaterial = material;
    }
    
    public void TakeEffectOn(ISkillRelated attackEntity, IAttackRelated sufferEntity)
    {
        if (attackEntity.GetFaction() != sufferEntity.GetFaction())
            if (sufferEntity.GetType() == typeof(CreatureEntity))
                sufferEntity.GetEffectComp().RecordFrozen(_duration, _effectMaterial);
    }
}