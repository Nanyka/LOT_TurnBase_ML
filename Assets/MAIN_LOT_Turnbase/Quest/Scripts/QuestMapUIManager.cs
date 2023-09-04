using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class QuestMapUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_ConfirmPanel;
        
        private IConfirmFunction m_CurrentConfirm;
        private Camera m_Camera;
        private int m_LayerMask = 1 << 8;

        private void Start()
        {
            m_Camera = Camera.main;
            QuestMapSavingManager.Instance.OnClickQuestButton.AddListener(ShowConfirmPanel);
        }
        
        
        protected virtual void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var moveRay = m_Camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(moveRay, out var moveHit, 100f, m_LayerMask))
                {
                    if (moveHit.collider.TryGetComponent(out QuestButton questButton))
                        ShowConfirmPanel(questButton);
                }
            }
        }

        private void ShowConfirmPanel(IConfirmFunction questButton)
        {
            m_ConfirmPanel.SetActive(true);
            m_CurrentConfirm = questButton;
        }

        public void OnConfirmQuest()
        {
            m_CurrentConfirm.ClickYes();
        }
    }
}