using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace JumpeeIsland
{
    public class AoeTutorial1 : MonoBehaviour, IOnTrackController
    {
        [SerializeField] private Transform factoryPos;
        [SerializeField] private Transform monsterPos;

        private void Start()
        {
            GameFlowManager.Instance.OnDataLoaded.AddListener(StartTimeline);
            TimelineManager.Instance.PauseTimeline(GetComponent<PlayableDirector>(), ButtonRequire.STARTGAME);
        }
        
        private void StartTimeline(long arg0)
        {
            // Debug.Log("Resume timeline 1");
            TimelineManager.Instance.ResumeTimeline(ButtonRequire.STARTGAME);
        }
        
        public void SetActive(bool isActive)
        {
            
        }

        public void Spawn()
        {
            throw new System.NotImplementedException();
        }

        public void ActionOne()
        {
            var factory = SavingSystemManager.Instance.GetEnvironmentData().BuildingData.First(t =>
                t.FactionType == FactionType.Player && t.BuildingType == BuildingType.TRAININGCAMP);
            factoryPos.position = Camera.main.WorldToScreenPoint(factory.Position);
        }

        public void ActionTwo()
        {
            var monsterPlace = SavingSystemManager.Instance.GetEnvironmentData().BuildingData.First(t =>
                t.FactionType == FactionType.Enemy && t.BuildingType == BuildingType.GUARDIANAREA);
            monsterPos.position = Camera.main.WorldToScreenPoint(monsterPlace.Position);
        }

        public void ActionThree()
        {
            throw new System.NotImplementedException();
        }
    }
}