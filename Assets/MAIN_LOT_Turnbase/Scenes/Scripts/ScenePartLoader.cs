using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace JumpeeIsland
{
    public class ScenePartLoader : MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private GameObject _backButton;
        [SerializeField] private bool isLoaded;

        void Start()
        {
            //verify if the scene is already open to avoid opening a scene twice
            if (SceneManager.sceneCount > 0)
            {
                for (int i = 0; i < SceneManager.sceneCount; ++i)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene.name == _sceneName)
                    {
                        isLoaded = true;
                    }
                }
            }
        }
        public void LoadScene()
        {
            if (!isLoaded)
            {
                SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
                _backButton.SetActive(true);
                isLoaded = true;
            }
        }

        public void UnLoadScene()
        {
            if (isLoaded)
            {
                SceneManager.UnloadSceneAsync(_sceneName);
                _backButton.SetActive(false);
                isLoaded = false;
            }
        }
    }
}