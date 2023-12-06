using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class AoeResearchMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI researchName;
        [SerializeField] private TextMeshProUGUI explaination;
        [SerializeField] private Image researchIcon;
        [SerializeField] private GameObject confirmPanel;

        private ILaboratory _curLaboratory;
        
        private void Start()
        {
            MainUI.Instance.OnAskForResearch.AddListener(ResearchRequest);
        }

        private void ResearchRequest(ILaboratory laboratory)
        {
            _curLaboratory = laboratory;
            var research = _curLaboratory.GetResearch();
            researchName.text = research.ResearchName;
            explaination.text = research.Explaination;
            researchIcon.sprite = AddressableManager.Instance.GetAddressableSprite(research.ResearchIcon);
            confirmPanel.SetActive(true);
        }

        public void ClickResearch()
        {
            _curLaboratory.ConductResearch();
        }

        public void ClickReject()
        {
            _curLaboratory.RejectResearch();
        }
    }
}