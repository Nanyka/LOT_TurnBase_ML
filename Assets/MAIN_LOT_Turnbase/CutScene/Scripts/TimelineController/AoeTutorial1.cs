using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace JumpeeIsland
{
    public class AoeTutorial1 : MonoBehaviour, IOnTrackController
    {
        [SerializeField] private Transform factoryObject;
        [SerializeField] private Transform factoryPos;
        [SerializeField] private AoeTutorialRegisterComp[] _registerComps;

        private int _currentRegister;

        private void Start()
        {
            GameFlowManager.Instance.OnDataLoaded.AddListener(StartTimeline);
            TimelineManager.Instance.PauseTimeline(GetComponent<PlayableDirector>(), ButtonRequire.STARTGAME);
        }
        
        private void StartTimeline(long arg0)
        {
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
            // var factory = SavingSystemManager.Instance.GetEnvironmentData().BuildingData.First(t =>
            //     t.FactionType == FactionType.Player && t.BuildingType == BuildingType.TRAININGCAMP);
            factoryPos.position = Camera.main.WorldToScreenPoint(factoryObject.position);
        }

        public void ActionTwo()
        {
            _registerComps[_currentRegister].Register();
            _currentRegister++;
        }

        public void ActionThree()
        {
            throw new System.NotImplementedException();
        }
    }
}