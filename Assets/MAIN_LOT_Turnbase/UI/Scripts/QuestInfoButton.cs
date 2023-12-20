using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class QuestInfoButton : MonoBehaviour
    {
        [SerializeField] private GameObject _infoPanel;
        [SerializeField] private StarHolder[] _stars;

        private Quest _currentQuest;

        private void Start()
        {
            MainUI.Instance.OnStarGuide.AddListener(ShowGuide);

            // _currentQuest = GameFlowManager.Instance.GetQuest();
            //
            // if (_currentQuest != null && _currentQuest.isFinalBoss)
            //     _infoPanel.SetActive(false);
            // else
            //     MainUI.Instance.OnEnableInteract.AddListener(EnableGuidePanel);
        }

        // private void EnableGuidePanel()
        // {
        //     _infoPanel.SetActive(true);
        // }

        private void ShowGuide(int starIndex, string guideMessage, bool isCompleted)
        {
            _stars[starIndex].EnableStar(guideMessage, isCompleted);
        }

        public void OnClickButton()
        {
            if (MainUI.Instance.IsInteractable == false)
                return;
            
            _infoPanel.SetActive(!_infoPanel.activeInHierarchy);
        }
    }
}