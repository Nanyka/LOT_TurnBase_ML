using JumpeeIsland;

public enum SkillEffectType
{
    None,
    StrengthBoost,
    Teleport,
    JumpBoost,
}

public interface SkillEffect
{
    public void TakeEffectOn(Entity attackEntity,Entity sufferEntity);
}