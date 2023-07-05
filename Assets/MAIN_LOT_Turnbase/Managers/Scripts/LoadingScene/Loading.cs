using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JumpeeIsland
{
    public class Loading : MonoBehaviour
    {
        [SerializeField] private string m_EcoModeScene;
        [SerializeField] private Slider m_LoadingSlider;

        private AsyncOperationHandle m_SceneHandle;

        void OnEnable()
        {
            // m_SceneHandle = Addressables.GetDownloadSizeAsync("Level_0" + GameManager.s_CurrentLevel);
            // m_SceneHandle.Completed += OnCheckSize;

            // m_SceneHandle = Addressables.DownloadDependenciesAsync("Level_0" + GameManager.s_CurrentLevel);
            // m_SceneHandle.Completed += OnSceneLoaded;

            m_SceneHandle =
                Addressables.LoadSceneAsync(m_EcoModeScene, UnityEngine.SceneManagement.LoadSceneMode.Single, true);
            m_SceneHandle.Completed += OnLoadScene;
        }

        private void OnLoadScene(AsyncOperationHandle obj)
        {
            Debug.Log(obj.Result);
        }

        private void OnDisable()
        {
            m_SceneHandle.Completed -= OnSceneLoaded;
        }

        private void OnSceneLoaded(AsyncOperationHandle obj)
        {
            // We show the UI button once the scene is successfully downloaded      
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Success load scene.");
            }

            if (obj.Status == AsyncOperationStatus.Failed)
            {
                Debug.Log(obj.DebugName);
            }
        }

        private void Update()
        {
            // We don't need to check for this value every single frame, and certainly not after the scene has been loaded
            m_LoadingSlider.value = m_SceneHandle.PercentComplete;
        }
    }
}