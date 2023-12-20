using JumpeeIsland;
using UnityEngine;

public class Frozen : ISkillEffect
{
    private readonly int _duration;
    private readonly Material _effectMaterial;

    public Frozen(int duration, Material material)
    {
        _duration = duration;
        _effectMaterial = material;
    }
    
    public void TakeEffectOn(ISkillCaster attackEntity, IAttackRelated sufferEntity)
    {
        if (attackEntity.GetFaction() != sufferEntity.GetFaction())
            sufferEntity.GetEffectComp().RecordFrozen(_duration, _effectMaterial);
    }
}