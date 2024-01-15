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
        [SerializeField] private Transform zombiePos;

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
            GameFlowManager.Instance.AskGlobalVfx(GlobalVfxType.FULLSCREENCONFETTI,Vector3.zero);
        }

        public void ActionThree()
        {
            MainUI.Instance.OnShowCurrencyVfx.Invoke("WOOD",5,zombiePos.position);
        }
    }
}