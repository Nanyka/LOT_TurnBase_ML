using JumpeeIsland;
using UnityEngine;

public class StrengthBooster : SkillEffect
{
    private int _duration;
    private int _magnitude;

    public StrengthBooster(int duration,int magnitude)
    {
        _duration = duration;
        _magnitude = magnitude;
    }
    
    public void TakeEffectOn(Entity attackEntity,Entity sufferEntity)
    {
        if (attackEntity.GetFaction() == sufferEntity.GetFaction())
        {
            if (sufferEntity.GetType() == typeof(CreatureEntity))
            {
                sufferEntity.GetEffectComp().AdjustStrength(_magnitude,_duration);
                // if (sufferEntity.GetEffectComp().CheckStrengthBoostState() == false)
                // {
                //     Debug.Log($"Boost strength of {attackEntity.name} during {_duration} steps.");
                //     var suffererData = (CreatureData)sufferEntity.GetData();
                //     suffererData.CurrentDamage *= _magnitude;
                //     suffererData.StregthBoostRemain = _duration;
                // }
            }
        }
    }
}