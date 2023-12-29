using UnityEngine;

namespace JumpeeIsland
{
	public class UIActiveChecker : MonoBehaviour, ITimeMachineChecker
	{
		[SerializeField] private GameObject checkedObject;
	
		public bool ConditionCheck()
		{
			return checkedObject.activeInHierarchy;
		}
	}
}