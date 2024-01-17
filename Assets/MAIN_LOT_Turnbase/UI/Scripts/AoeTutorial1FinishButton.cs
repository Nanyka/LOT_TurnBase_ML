using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JumpeeIsland
{
    public class AoeTutorial1FinishButton : MonoBehaviour
    {
        [SerializeField] private string m_ModeScene;

        private AsyncOperationHandle m_SceneHandle;
        private bool _isClicked;

        public void OnClickChangeMode()
        {
            if (_isClicked) return;

            if (MainUI.Instance != null)
                MainUI.Instance.IsInteractable = false;

            SavingSystemManager.Instance.SaveBossUnlock(1);
            SavingSystemManager.Instance.GetEnvDataForSave().mapSize = 1;
            SavingSystemManager.Instance.SaveBossBattle();

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