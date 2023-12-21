using System;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public class JILocalSaveManager : MonoBehaviour, ILocalSaver
    {
        [SerializeField] private EnvironmentData _envData;
        private string _gamePath;
        private bool encrypt = true;

        private void Awake()
        {
            _gamePath = Application.persistentDataPath;
        }

        public void SavePlayerEnv(EnvironmentData envData)
        {
            // _envData = envData;
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Save(envData, envPath, EnvDataWasSaved, encrypt);
        }

        private async void EnvDataWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
            {
                Debug.LogError($"Error saving data:\n{result}\n{message}");
            }

            // if (result == SaveResult.Success)
            // {
            //     await LoadEnvironment();
            // }
        }
        
        public void LoadEnvironment()
        {
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Load<EnvironmentData>(envPath, EnvWasLoaded, encrypt);
        }
        
        private void EnvWasLoaded(EnvironmentData data, SaveResult result, string message)
        {
            Debug.Log($"Env Was Loaded:\n{result}\n{message}");
            
            if (result == SaveResult.EmptyData || result == SaveResult.Error)
            {
                Debug.LogError("No Env data File Found -> Creating new data...");
            }
        
            if (result == SaveResult.Success)
            {
                _envData = data;
            }
        }

        public EnvironmentData GetEnvData()
        {
            return _envData;
        }
        
        private string GetSavingPath(SavingPath tailPath)
        {
            var envPath = _gamePath + "/" + tailPath;
            return envPath;
        }
    }

    public interface ILocalSaver
    {
        public EnvironmentData GetEnvData();
    }
}