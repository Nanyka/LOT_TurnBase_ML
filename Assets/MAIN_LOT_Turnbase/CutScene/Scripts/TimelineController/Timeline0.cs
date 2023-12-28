using UnityEngine;
using UnityEngine.Playables;

namespace JumpeeIsland
{
    public class Timeline0 : MonoBehaviour, IOnTrackController
    {
        private PlayableDirector _playableDirector;
        private IConfirmFunction _confirmFunction;

        private void Start()
        {
            MainUI.Instance.OnTurnToBattleMode.AddListener(SetConfirmFunction);
            GameFlowManager.Instance.OnDataLoaded.AddListener(StartTimeline);

            TimelineManager.Instance.PauseTimeline(GetComponent<PlayableDirector>(), ButtonRequire.STARTGAME);
        }

        private void SetConfirmFunction(IConfirmFunction confirmFunction)
        {
            _confirmFunction = confirmFunction;
        }

        private void StartTimeline(long arg0)
        {
            Debug.Log("Resume timeline");
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
            _confirmFunction.ClickYes();
        }

        public void ActionTwo()
        {
            throw new System.NotImplementedException();
        }

        public void ActionThree()
        {
            throw new System.NotImplementedException();
        }
    }
}