using UnityEngine;

namespace JumpeeIsland
{
    public class ViewPositionChecker : MonoBehaviour, ITimeMachineChecker
    {
        [SerializeField] private Transform _checkedObject;
        [SerializeField] private Vector3 _referencePos;
        [SerializeField] private float _checkRange;
        [SerializeField] private bool _isOutOfRangeCheck;
        
        public bool ConditionCheck()
        {
            if (_isOutOfRangeCheck)
                return Vector3.Distance(_checkedObject.position, _referencePos) > _checkRange;
            else
                return Vector3.Distance(_checkedObject.position, _referencePos) < _checkRange;
        }
    }
}