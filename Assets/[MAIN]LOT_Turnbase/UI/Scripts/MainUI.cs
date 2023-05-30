using System;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class MainUI : Singleton<MainUI>
    {
        [NonSerialized] public UnityEvent<IGetCreatureInfo> OnShowCreatureInfo = new(); // send to UIManager; invoke at Units
        [NonSerialized] public UnityEvent<long> OnRemainStep = new(); // send to UIManager; invoke at UnitManagers
        [NonSerialized] public UnityEvent OnClickIdleButton = new(); // send to PlayerFactionManager; invoke at IdleButton & MovingPath
        [NonSerialized] public UnityEvent<int> OnGameOver = new(); // send to GameOverAnnouncer; invoke at PlayerFactionManager
    }
}
