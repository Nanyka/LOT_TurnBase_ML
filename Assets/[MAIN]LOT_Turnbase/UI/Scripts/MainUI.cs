using System;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class MainUI : Singleton<MainUI>
    {
        [NonSerialized] public UnityEvent<IGetCreatureInfo> OnShowCreatureInfo = new(); // send to CreatureInfoUI; invoke at CreatureInGame
        [NonSerialized] public UnityEvent<long> OnRemainStep = new(); // send to StepCounter; invoke at EnvironmentManager
        [NonSerialized] public UnityEvent OnUpdateCurrencies = new(); // send to CurrenciesInfo; invoke at SavingSystemManager
        [NonSerialized] public UnityEvent OnClickIdleButton = new(); // send to PlayerFactionManager; invoke at DontMoveButton & MovingPath
        [NonSerialized] public UnityEvent<FactionType> OnGameOver = new(); // send to GameOverAnnouncer; invoke at PlayerFactionManager
    }
}
