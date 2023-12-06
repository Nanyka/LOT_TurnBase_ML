using System;

namespace JumpeeIsland
{
    [Serializable]
    public class Research
    {
        // Type of the research: For troop, for worker (or worker is a kind of troop), or a spell
        // If it's for troop:
        //  *If it is not a troop transformation, or troopType != NONE (we use level as the index of troop transformation)
        //      What type of troop is the target of this research?
        //      --> This will be TROOP_STATS research
        //  *If it is for troop transformation
        //      Which troop?
        //      --> This will be TROOP_TRANSFORM research
        // If it's not for troop:
        //      --> This will be SPELL research which mostly intervene the battlefield by a particle system
        // Research is loaded from troops in inventory (TROOP_LEVEL)
        // and MainHallTier remote config (TROOP_STATS & SPELL)

        public string ResearchName = "Research";
        public string ResearchIcon;
        public ResearchType ResearchType;
        public TroopType TroopType;
        public TroopStats TroopStats;
        public string Target;
        public int Magnitude;
        public int Difficulty; // Difficulty of the research show cost to conduct this research
        public string Explaination;
    }

    public enum ResearchType
    {
        NONE,
        TROOP_STATS,
        TROOP_TRANSFORM,
        SPELL
    }

    public enum TroopType
    {
        NONE,
        INFANTRY,
        CAVALRY,
        ARTILLERY,
        ARMORED
    }

    public enum TroopStats
    {
        NONE,
        HP,
        POWER,
        SPEED,
        STRONG
    }
}