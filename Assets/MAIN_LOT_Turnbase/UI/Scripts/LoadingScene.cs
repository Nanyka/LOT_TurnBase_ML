using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class LoadingScene : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingScene;
        
        private void Start()
        {
            MainUI.Instance.OnShowDropTroopMenu.AddListener(HideLoadingScene);
        }

        private void HideLoadingScene(List<CreatureData> arg0)
        {
            _loadingScene.SetActive(false);
        }
    }
}