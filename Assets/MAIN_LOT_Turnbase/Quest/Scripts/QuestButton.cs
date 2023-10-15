using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JumpeeIsland
{
    public class QuestButton : MonoBehaviour, IConfirmFunction
    {
        public string questAddress;
        public string bossMap;

        [SerializeField] private MeshRenderer[] _stars;
        
        private AsyncOperationHandle m_SceneHandle;

        public void Init(QuestUnit questUnit)
        {
            if (questAddress.Equals(questUnit.QuestAddress))
            {
                if (_stars.Length == 0)
                    return;
                
                for (int i = 0; i < questUnit.StarAmount; i++)
                    _stars[i].material = AddressableManager.Instance.GetMaterial("/Materials/Yellow");

                if (questUnit.StarAmount == 3)
                    GetComponent<Collider>().enabled = false;
            }
        }
        
        public void ClickYes()
        {
            QuestMapSavingManager.Instance.OnSetQuestSceneName.Invoke(questAddress);
            m_SceneHandle = Addressables.LoadSceneAsync(bossMap);
            m_SceneHandle.Completed += OnLoadScene;
        }
        
        private void OnLoadScene(AsyncOperationHandle obj)
        {
            Debug.Log(obj.Result);
        }

        public Entity GetEntity()
        {
            throw new System.NotImplementedException();
        }
    }
}