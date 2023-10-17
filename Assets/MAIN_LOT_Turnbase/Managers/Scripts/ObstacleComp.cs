using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class ObstacleComp : MonoBehaviour
    {
        [SerializeField] private FactionType _factionType;
        
        private void OnEnable()
        {
            GameFlowManager.Instance.OnDomainRegister.Invoke(gameObject, _factionType);
        }

        private void OnDisable()
        {
            GameFlowManager.Instance.OnDomainRemover.Invoke(gameObject, _factionType);
        }
    }
}