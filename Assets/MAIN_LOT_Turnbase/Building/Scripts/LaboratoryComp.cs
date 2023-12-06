using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JumpeeIsland
{
    public class LaboratoryComp : MonoBehaviour, IInputExecutor, ILaboratory
    {
        [SerializeField] private GameObject topicPanel;
        [SerializeField] private float researchTime = 3f;

        private Research m_Research;
        private bool _isOnHolding;
        private bool _isTopicLoaded;

        private void OnEnable()
        {
            GetComponent<IBuildingConstruct>().GetCompletedEvent().AddListener(LoadResearch);
        }

        private void OnDisable()
        {
            CancelInvoke();
            GetComponent<IBuildingConstruct>().GetCompletedEvent().RemoveListener(LoadResearch);
        }

        // TODO: Load research on UI and decide to conduct or not
        public void OnClick()
        {
            if (_isTopicLoaded)
            {
                Debug.Log($"Click on {name}");
                MainUI.Instance.OnAskForResearch.Invoke(this);
            }
        }

        public void OnHoldEnter()
        {
            Debug.Log($"Hold on {name}");
            _isOnHolding = true;
        }

        public void OnHolding(Vector3 position)
        {
            Debug.Log($"On holding");
        }

        public void OnHoldCanCel()
        {
            if (_isOnHolding == false)
                return;

            _isOnHolding = false;
            Debug.Log("On hold cancel");
        }

        public void OnDoubleTaps()
        {
            Debug.Log("On double tab");
        }

        public void ConductResearch()
        {
            Debug.Log("Execute the research");
            //TODO: apply the research on relevant entities
            LoadResearch();
        }

        public void RejectResearch()
        {
            Debug.Log("Reject research");
            LoadResearch();
        }

        private void LoadResearch()
        {
            topicPanel.SetActive(false);
            _isTopicLoaded = false;
            Invoke(nameof(SelectResearch), researchTime);
        }
        
        private void SelectResearch()
        {
            Debug.Log("Load research");
            if (_isTopicLoaded)
                return;

            var topics = SavingSystemManager.Instance.GetResearchTopics();
            m_Research = topics.ElementAt(Random.Range(0, topics.Count()));
            topicPanel.SetActive(true);
            _isTopicLoaded = true;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Research GetResearch()
        {
            return m_Research;
        }
    }

    public interface ILaboratory
    {
        public Research GetResearch();
        public void ConductResearch();
        public void RejectResearch();
    }
}