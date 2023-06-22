using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [CreateAssetMenu(fileName = "Tutorial", menuName = "JumpeeIsland/Tutorial", order = 6)]
    public class Tutorial : ScriptableObject
    {
        [SerializeField] private List<GameMasterCondition> conditions;
        [SerializeField] private List<TutorialStep> steps;
        [SerializeField] private string nextTutorial;
        
        public bool CheckExecute()
        {
            foreach (var condition in conditions)
                if (condition.PassCondition() == false)
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
    }
}