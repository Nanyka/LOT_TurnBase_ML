using UnityEngine;

public class TimeMachineCheckableObject: MonoBehaviour, ITimeMachineChecker
{
    public bool ConditionCheck()
    {
        return true;
    }
}

public interface ITimeMachineChecker
{
    public bool ConditionCheck();
}