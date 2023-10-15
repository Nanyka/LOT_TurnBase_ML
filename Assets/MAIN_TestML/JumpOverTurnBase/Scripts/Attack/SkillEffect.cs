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
    public void TakeEffectOn(Entity attackEntity,Entity sufferEntity);
}