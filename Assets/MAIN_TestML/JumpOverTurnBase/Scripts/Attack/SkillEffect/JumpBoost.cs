using JumpeeIsland;
using UnityEngine;

public class JumpBoost : ISkillEffect
{
    private int _duration;
    private int _magnitude;

    public JumpBoost(int duration, int magnitude)
    {
        _duration = duration;
        _magnitude = magnitude;
    }

    public void TakeEffectOn(ISkillCaster attackEntity, IAttackRelated sufferEntity)
    {
        if (sufferEntity.GetType() == typeof(CreatureEntity))
            sufferEntity.GetEffectComp().JumpBoost(_duration, _magnitude);
    }
}