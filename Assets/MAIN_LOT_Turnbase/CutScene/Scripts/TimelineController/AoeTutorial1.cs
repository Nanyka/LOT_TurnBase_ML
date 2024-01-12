using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class AoeTutorial1 : MonoBehaviour, IOnTrackController
    {
        [SerializeField] private Transform factoryObject;
        [SerializeField] private Transform factoryPos;
        [SerializeField] private AoeTutorialEntity[] _tutorialEntities;

        private int _curIndex;

        private void Start()
        {
            GameFlowManager.Instance.OnDataLoaded.AddListener(StartTimeline);
            TimelineManager.Instance.PauseTimeline(GetComponent<PlayableDirector>(), ButtonRequire.STARTGAME);
        }
        
        private void StartTimeline(long arg0)
        {
            TimelineManager.Instance.ResumeTimeline(ButtonRequire.STARTGAME);
        }

        public void SetIntParam(int intParam)
        {
            _curIndex = intParam;
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
            // var factory = SavingSystemManager.Instance.GetEnvironmentData().BuildingData.First(t =>
            //     t.FactionType == FactionType.Player && t.BuildingType == BuildingType.TRAININGCAMP);
            factoryPos.position = Camera.main.WorldToScreenPoint(factoryObject.position);
        }

        public void ActionTwo()
        {
            // _tutorialEntities[_curIndex].TakeDamage();
        }

        public void ActionThree()
        {
            // _tutorialEntities[_curIndex].Die();
        }
    }
}