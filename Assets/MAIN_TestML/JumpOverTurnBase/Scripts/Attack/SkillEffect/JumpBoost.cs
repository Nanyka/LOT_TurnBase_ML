using JumpeeIsland;
using UnityEngine;

public class JumpBoost : SkillEffect
{
    private int _duration;
    private int _magnitude;

    public JumpBoost(int duration, int magnitude)
    {
        _duration = duration;
        _magnitude = magnitude;
    }

    public void TakeEffectOn(Entity attackEntity, Entity sufferEntity)
    {
        if (sufferEntity.GetType() == typeof(CreatureEntity))
        {
            Debug.Log($"Jump boost of {sufferEntity.name} during {_duration} steps.");
            sufferEntity.GetEffectComp().JumpBoost(_duration, _magnitude);
        }
    }
}