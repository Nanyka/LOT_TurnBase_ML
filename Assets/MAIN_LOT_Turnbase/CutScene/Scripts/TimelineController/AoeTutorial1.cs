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

        private int _curInteger;
        private string _curString;

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
            _curInteger = intParam;
        }

        public void SetStringParam(string stringParam)
        {
            _curString = stringParam;
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
            factoryPos.position = Camera.main.WorldToScreenPoint(factoryObject.position);
        }

        public void ActionTwo()
        {
            GameFlowManager.Instance.AskGlobalVfx(GlobalVfxType.FULLSCREENCONFETTI, Vector3.zero);
        }

        public void ActionThree()
        {
            if (_curInteger > 0)
            {
                SavingSystemManager.Instance.IncreaseLocalCurrency(_curString,_curInteger);
                MainUI.Instance.OnShowCurrencyVfx.Invoke(_curString, _curInteger, zombiePos.position);
            }
            else
            {
                SavingSystemManager.Instance.DeductCurrency(_curString,_curInteger);
            }
        }
    }
}