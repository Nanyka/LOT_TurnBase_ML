using JumpeeIsland;

public enum SkillEffectType
{
    None,
    StrengthBoost,
    Teleport,
    JumpBoost,
    Frozen
}

public interface ISkillEffect
{
    public void TakeEffectOn(ISkillCaster attackEntity,IAttackRelated sufferEntity);
}