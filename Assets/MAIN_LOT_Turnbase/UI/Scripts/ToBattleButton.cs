using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JumpeeIsland
{
    public class ToBattleButton : MonoBehaviour
    {
        [SerializeField] private string m_BattleModeScene;

        private AsyncOperationHandle m_SceneHandle;
        private bool _isClicked;

        public void OnClickBattleButton()
        {
            if (_isClicked) return;

            MainUI.Instance.IsInteractable = false;
            _isClicked = true;
            m_SceneHandle = Addressables.LoadSceneAsync(m_BattleModeScene);
            m_SceneHandle.Completed += OnLoadScene;
        }

        private void OnLoadScene(AsyncOperationHandle obj)
        {
            Debug.Log(obj.Result);
        }

        // private void OnDisable()
        // {
        //     m_SceneHandle.Completed -= OnSceneLoaded;
        // }

        // private void OnSceneLoaded(AsyncOperationHandle obj)
        // {
        //     // We show the UI button once the scene is successfully downloaded      
        //     if (obj.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         Debug.Log("Success load battle scene.");
        //     }
        //
        //     if (obj.Status == AsyncOperationStatus.Failed)
        //     {
        //         Debug.Log(obj.DebugName);
        //     }
        // }
    }
}