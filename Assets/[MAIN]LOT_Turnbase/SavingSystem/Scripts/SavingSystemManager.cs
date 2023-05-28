using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

namespace JumpeeIsland
{
    public enum SavingPath
    {
        PlayerEnvData
    }
    
    public class SavingSystemManager : Singleton<SavingSystemManager>
    {
        // Save Player environment data whenever a creature move
        [NonSerialized] public UnityEvent OnSavePlayerEnvData = new(); // invoke at CreatureEntity

        private EnvironmentLoader _envLoader;
        
        private EnvironmentData _environmentData;
        private string _gamePath;
        private bool encrypt = true;

        protected override void Awake()
        {
            base.Awake();
            _gamePath = Application.persistentDataPath;
        }

        private void Start()
        {
            _envLoader = GetComponent<EnvironmentLoader>();
            OnSavePlayerEnvData.AddListener(SavePlayerEnv);
        }

        #region ENVIRONMENT
        
        // SAVE function
        private void SavePlayerEnv()
        {
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Save(_envLoader.GetData(),envPath,DataWasSaved,encrypt);
        }

        private void DataWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
            {
                Debug.LogError($"Error saving data:\n{result}\n{message}");
            }
        }
        
        // LOAD function
        public EnvironmentData LoadEnvironment()
        {
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Load<EnvironmentData>(envPath, DataWasLoaded, encrypt);
            return _environmentData;
        }

        private void DataWasLoaded(EnvironmentData data, SaveResult result, string message)
        {
            Debug.Log($"Data Was Loaded:\n{result}\n{message}");

            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No Data File Found -> Creating new data...");

            if (result == SaveResult.Success)
                _environmentData = data;
        }

        #endregion

        private string GetSavingPath(SavingPath tailPath)
        {
            var envPath = _gamePath + "/" + tailPath;
            return envPath;
        }
    }
}
