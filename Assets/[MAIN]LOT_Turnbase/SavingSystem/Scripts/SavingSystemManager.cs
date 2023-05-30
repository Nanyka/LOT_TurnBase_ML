using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

namespace JumpeeIsland
{
    public enum SavingPath
    {
        PlayerEnvData,
        Currencies
    }

    [RequireComponent(typeof(EnvironmentLoader))]
    [RequireComponent(typeof(CurrencyLoader))]
    public class SavingSystemManager : Singleton<SavingSystemManager>
    {
        // Save Player environment data whenever a creature move
        [NonSerialized] public UnityEvent OnSavePlayerEnvData = new(); // invoke at CreatureEntity

        [SerializeField] private JICloudConnector _cloudConnector;
        private EnvironmentLoader _envLoader;
        private CurrencyLoader _currencyLoader;

        // private EnvironmentData _environmentData;
        private string _gamePath;
        private bool encrypt = true;

        protected override void Awake()
        {
            base.Awake();
            _gamePath = Application.persistentDataPath;
            _envLoader = GetComponent<EnvironmentLoader>();
            _currencyLoader = GetComponent<CurrencyLoader>();
            OnSavePlayerEnvData.AddListener(SavePlayerEnv);
            StartUpProcessor.Instance.OnLoadData.AddListener(StartUpLoadData);
        }

        private async void StartUpLoadData()
        {
            await LoadEnvironment();
            _envLoader.Init();

            await LoadCurrencies();
            _currencyLoader.Init();
        }

        #region ENVIRONMENT

        // SAVE function
        private void SavePlayerEnv()
        {
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Save(_envLoader.GetData(), envPath, DataWasSaved, encrypt);
        }

        private void DataWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
            {
                Debug.LogError($"Error saving data:\n{result}\n{message}");
            }
        }

        private async Task LoadEnvironment()
        {
            // Authenticate on UGS and get envData
            await _cloudConnector.Init();
            _envLoader.SetData(await _cloudConnector.OnLoadEnvData());
            
            var envPath = GetSavingPath(SavingPath.PlayerEnvData);
            SaveManager.Instance.Load<EnvironmentData>(envPath, DataWasLoaded, encrypt);
        }

        private void DataWasLoaded(EnvironmentData data, SaveResult result, string message)
        {
            Debug.Log($"Data Was Loaded:\n{result}\n{message}");

            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No Data File Found -> Creating new data...");

            if (result == SaveResult.Success)
            {
                // Compare timestamp. Select the latest data
                _envLoader.SetData(_envLoader.GetData().timestamp > data.timestamp ? _envLoader.GetData() : data);
            }
        }

        #endregion

        #region CURRENCIES

        private async Task LoadCurrencies()
        {
            // Authenticate on UGS and get envData
            _currencyLoader.SetData(await _cloudConnector.OnLoadCurrency());
            
            // var envPath = GetSavingPath(SavingPath.Currencies);
            // SaveManager.Instance.Load<EnvironmentData>(envPath, DataWasLoaded, encrypt);
        }

        #endregion

        #region GET & SET
        
        private string GetSavingPath(SavingPath tailPath)
        {
            var envPath = _gamePath + "/" + tailPath;
            return envPath;
        }

        public EnvironmentData GetEnvironmentData()
        {
            return _envLoader.GetData();
        }

        #endregion
    }
}