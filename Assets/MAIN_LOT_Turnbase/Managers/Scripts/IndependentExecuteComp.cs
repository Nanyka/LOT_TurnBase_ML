using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class IndependentExecuteComp : MonoBehaviour
    {
        [SerializeField] private int[] _animIndex;
        [SerializeField] private FactionType _executeFaction;

        private AnimateComp _animateComp;
        
        private void Start()
        {
            _animateComp = GetComponent<AnimateComp>();
            GameFlowManager.Instance.GetEnvManager().OnChangeFaction.AddListener(ExecuteAttackAnim);
        }

        private void ExecuteAttackAnim()
        {
            if (GameFlowManager.Instance.GetEnvManager().GetCurrFaction() == _executeFaction)
                _animateComp.TriggerAttackAnim(_animIndex[Random.Range(0,_animIndex.Length)]);
        }

        private void OnDisable()
        {
            GameFlowManager.Instance?.GetEnvManager().OnChangeFaction.RemoveListener(ExecuteAttackAnim);
        }
    }
}