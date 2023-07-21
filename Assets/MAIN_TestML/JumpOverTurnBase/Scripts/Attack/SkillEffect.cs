using JumpeeIsland;
using UnityEngine;

public enum SkillEffectType
{
    None,
    StrengthBoost,
}

public interface SkillEffect
{
    public void TakeEffectOn(Entity attackEntity,Entity sufferEntity);
}

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
                if (sufferEntity.GetEffectComp().StrengthBoost())
                {
                    Debug.Log($"Boost strength of {attackEntity.name} during {_duration} steps.");
                    var suffererData = (CreatureData)sufferEntity.GetData();
                    suffererData.CurrentDamage *= _magnitude;
                    suffererData.StregthBoostRemain = _duration;
                }
            }
        }
    }
}