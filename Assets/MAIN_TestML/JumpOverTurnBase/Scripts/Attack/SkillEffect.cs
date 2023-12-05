using JumpeeIsland;

public enum SkillEffectType
{
    None,
    StrengthBoost,
    Teleport,
    JumpBoost,
    Frozen
}

public interface SkillEffect
{
    public void TakeEffectOn(ISkillRelated attackEntity,IAttackRelated sufferEntity);
}