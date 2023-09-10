using System;
using UnityEngine;
using WebSocketSharp;

namespace JumpeeIsland
{
    public class QuestFlowManager : GameFlowManager
    {
        [Header("Test phase")] [SerializeField]
        private bool _isTestPhase;

        [SerializeField] private Quest _testQuest;

        private Quest _quest;

        // private QuestData m_QuestData;
        private bool encrypt = true;
        private string _gamePath;

        protected override void Start()
        {
            base.Start();
            _gamePath = Application.persistentDataPath;
            LoadQuestData();
        }

        protected override void ConfirmGameStarted()
        {
            base.ConfirmGameStarted();
            if (_quest.targetPos.x.Equals(float.NegativeInfinity))
                return;

            var target = _environmentManager.GetObjectByPosition(_quest.targetPos, FactionType.Enemy);
            if (_quest.targetType == EntityType.RESOURCE)
                target = _environmentManager.GetObjectByPosition(_quest.targetPos, FactionType.Neutral);

            if (target != null)
                target.AddComponent<EndGameComp>();

            // Load tutorial if any
            LoadTutorialManager(_quest.tutorialForQuest);
        }

        public override QuestData GetQuestData()
        {
            return SavingSystemManager.Instance.GetQuestData();
        }

        public override Quest GetQuest()
        {
            if (_isTestPhase)
                return _quest = _testQuest;

            _quest = AddressableManager.Instance.GetAddressableSO(SavingSystemManager.Instance.GetQuestData()
                .CurrentQuestAddress) as Quest;
            return _quest;
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
                SavingSystemManager.Instance.SetQuestData(questData);
        }

        private string GetSavingPath(SavingPath tailPath)
        {
            var envPath = _gamePath + "/" + tailPath;
            return envPath;
        }
    }
}