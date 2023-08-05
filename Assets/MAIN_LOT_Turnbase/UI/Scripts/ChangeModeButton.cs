using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JumpeeIsland
{
    public class ChangeModeButton : MonoBehaviour
    {
        [SerializeField] private string m_ModeScene;

        private AsyncOperationHandle m_SceneHandle;
        private bool _isClicked;

        public void OnClickChangeMode()
        {
            if (_isClicked) return;

            MainUI.Instance.IsInteractable = false;
            _isClicked = true;
            m_SceneHandle = Addressables.LoadSceneAsync(m_ModeScene);
            m_SceneHandle.Completed += OnLoadScene;
        }

        private void OnLoadScene(AsyncOperationHandle obj)
        {
            Debug.Log(obj.Result);
        }
    }
}