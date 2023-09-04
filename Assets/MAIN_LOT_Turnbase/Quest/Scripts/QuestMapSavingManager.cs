using System;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    public class QuestMapSavingManager : Singleton<QuestMapSavingManager>
    {
        // invoke at QuestButton
        [NonSerialized] public UnityEvent<string> OnSetQuestAddress = new();
        
        // invoke at QuestButton
        [NonSerialized] public UnityEvent<IConfirmFunction> OnClickQuestButton = new();

        private QuestData m_QuestData = new();
        private bool encrypt = true;
        private string _gamePath;

        private void Start()
        {
            _gamePath = Application.persistentDataPath;
            OnSetQuestAddress.AddListener(SetQuestAddress);
        }

        private void SaveQuestData()
        {
            var gameStatePath = GetSavingPath(SavingPath.QuestData);
            SaveManager.Instance.Save(m_QuestData, gameStatePath, QuestDataWasSaved, encrypt);
        }
        
        private void QuestDataWasSaved(SaveResult result, string message)
        {
            if (result == SaveResult.Error)
                Debug.LogError($"Error saving quest data:\n{result}\n{message}");
        }
        
        private void LoadQuestData()
        {
            var gameStatePath = GetSavingPath(SavingPath.QuestData);
            SaveManager.Instance.Load<QuestData>(gameStatePath, QuestDataWasLoaded, encrypt);
        }

        private void QuestDataWasLoaded(QuestData questData, SaveResult result, string message)
        {
            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No State data File Found -> Creating new data...");

            if (result == SaveResult.Success)
                m_QuestData = questData;
        }

        public void SetQuestAddress(string questAddress)
        {
            m_QuestData.QuestAddress = questAddress;
            SaveQuestData();
        }
        
        private string GetSavingPath(SavingPath tailPath)
        {
            var envPath = _gamePath + "/" + tailPath;
            return envPath;
        }
    }
}