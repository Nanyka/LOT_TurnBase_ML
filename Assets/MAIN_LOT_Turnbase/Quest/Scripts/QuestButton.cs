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
        private AsyncOperationHandle m_SceneHandle;

        public void ClickYes()
        {
            QuestMapSavingManager.Instance.OnSetQuestAddress.Invoke(questAddress);
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