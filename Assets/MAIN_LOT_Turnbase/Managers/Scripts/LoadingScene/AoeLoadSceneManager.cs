using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeLoadSceneManager : MonoBehaviour
    {
        private async void Start()
        {
            var cloudConnector = JICloudConnector.Instance;
            await cloudConnector.Init();
            GetComponent<ILoadScene>().LoadScene();
        }
    }
}