using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

[Serializable]
public class TimeMachineBehaviour : PlayableBehaviour
{
	public TimeMachineAction action;
	public Condition condition;
	public string markerToJumpTo, markerLabel;
	public float timeToJumpTo;
    public GameObject checkingObject;

    private ITimeMachineChecker checker;

	[HideInInspector]
	public bool clipExecuted = false; //the user shouldn't author this, the Mixer does

	public bool ConditionMet()
	{
		switch(condition)
		{
			case Condition.Always:
				return true;
				
			case Condition.CustomCondition:
				//The Timeline will jump to the label or time if a specific Platoon still has at least 1 unit alive
				if(checkingObject.TryGetComponent(out checker))
				{
					return !checker.ConditionCheck();
				}

				return false;

			case Condition.Never:
			default:
				return false;
		}
	}

	public enum TimeMachineAction
	{
		Marker,
		JumpToTime,
		JumpToMarker,
		Pause,
	}

	public enum Condition
	{
		Always,
		Never,
		CustomCondition,
	}
}