using UnityEngine;

namespace JumpeeIsland
{
    public class ViewPositionChecker : MonoBehaviour, ITimeMachineChecker
    {
        [SerializeField] private Transform _checkedObject;
        [SerializeField] private Vector3 _referencePos;
        
        public bool ConditionCheck()
        {
            return Vector3.Distance(_checkedObject.position, _referencePos) < 2f;
        }
    }
}