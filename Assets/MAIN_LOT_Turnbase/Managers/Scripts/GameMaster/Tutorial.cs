using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "Tutorial", menuName = "JumpeeIsland/Tutorial", order = 6)]
    public class Tutorial : ScriptableObject
    {
        [SerializeField] private bool UsePassCondition;
        [ShowIf("UsePassCondition")]
        [SerializeField] private GameMasterCondition PassCondition;
        [SerializeField] private List<GameMasterCondition> conditions;
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private string nextTutorial;
        
        public bool CheckExecute()
        {
            foreach (var condition in conditions)
                if (condition.CheckPass() == false)
                    return false;

            return true;
        }

        public TutorialStep GetStep(int index)
        {
            return steps[index];
        }

        public bool CheckRunOutOfStep(int currentIndex)
        {
            return currentIndex >= steps.Count;
        }

        public string GetNextTutorial()
        {
            return nextTutorial;
        }

        public bool CheckPassCondition()
        {
            if (UsePassCondition == false)
                return false;
            
            return PassCondition.CheckPass();
        }

        public int GetStepAmount()
        {
            return steps.Count;
        }
    }
}