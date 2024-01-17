using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;

namespace JumpeeIsland
{
    public class StartBossMapCutScene : MonoBehaviour, IOnTrackController
    {
        private IConfirmFunction _confirmFunction;
        private PlayableDirector _playableDirector;

        private void Start()
        {
            _playableDirector = GetComponent<PlayableDirector>();
            
            MainUI.Instance.OnTurnToBattleMode.AddListener(SetConfirmFunction);
        }

        private void SetConfirmFunction(IConfirmFunction confirmFunction)
        {
            _confirmFunction = confirmFunction;
            _playableDirector.Play();
        }

        public void SetIntParam(int intParam)
        {
            
        }

        public void SetStringParam(string stringParam)
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