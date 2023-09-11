using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class JIAttackPath : MonoBehaviour
    {
        [SerializeField] protected SelectionCircle[] _attackPoints;
    
        public void AttackAt(IEnumerable<Vector3> highlightPos)
        {
            DisableAttackPath();
            int index = 0;
            foreach (var targetPos in highlightPos)
                _attackPoints[index].SwitchProjector(targetPos, index++);

            StartCoroutine(WaitToTurnOff());
        }

        private IEnumerator WaitToTurnOff()
        {
            yield return new WaitForSeconds(1f);
            DisableAttackPath();
        }

        private void DisableAttackPath()
        {
            foreach (var circle in _attackPoints)
                circle.SwitchProjector(false);
        }
    }
}