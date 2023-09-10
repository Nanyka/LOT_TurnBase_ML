using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class QuestMapUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject m_ConfirmPanel;
        [SerializeField] private int m_BossIndex;
        [SerializeField] private List<QuestButton> m_QuestButtons;

        private IConfirmFunction m_CurrentConfirm;
        private Camera m_Camera;
        private int m_LayerMask = 1 << 8;

        private void Start()
        {
            m_Camera = Camera.main;
            QuestMapSavingManager.Instance.OnUpdateQuestState.AddListener(Init);
            QuestMapSavingManager.Instance.OnClickQuestButton.AddListener(ShowConfirmPanel);
        }

        private void Init(QuestData questData)
        {
            if (questData.QuestChains == null || questData.QuestChains.Count == 0)
                return;
            
            foreach (var button in m_QuestButtons)
                foreach (var quest in questData.QuestChains[m_BossIndex].QuestUnits)
                {
                    button.Init(quest);
                }
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