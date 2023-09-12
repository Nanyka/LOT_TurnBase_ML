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

        [SerializeField] private Quest _quest;
        private bool _encrypt = true;
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

            GameObject target = null;

            switch (_quest.targetType)
            {
                case EntityType.ENEMY:
                    target = _environmentManager.GetObjectByPosition(_quest.targetPos, FactionType.Enemy);
                    break;
                case EntityType.RESOURCE:
                    target = _environmentManager.GetObjectByPosition(_quest.targetPos, FactionType.Neutral);
                    break;
                case EntityType.COLLECTABLE:
                {
                    var collectableController = FindObjectOfType<CollectableController>();
                    target = collectableController.GetCollectableByPos(_quest.targetPos).gameObject;
                }
                    break;
            }
            
            if (target != null)
                target.AddComponent<EndGameComp>();
        }

        public override QuestData GetQuestData()
        {
            return SavingSystemManager.Instance.GetQuestData();
        }

        public override Quest GetQuest()
        {
            return _quest;
        }

        private void LoadQuestData()
        {
            var gameStatePath = GetSavingPath(SavingPath.QuestData);
            SaveManager.Instance.Load<QuestData>(gameStatePath, QuestDataWasLoaded, _encrypt);
        }

        private void QuestDataWasLoaded(QuestData questData, SaveResult result, string message)
        {
            if (result == SaveResult.EmptyData || result == SaveResult.Error)
                Debug.LogError("No State data File Found -> Creating new data...");

            if (result == SaveResult.Success)
            {
                SavingSystemManager.Instance.SetQuestData(questData);

                if (_isTestPhase)
                    _quest = _testQuest;
                else
                    _quest = AddressableManager.Instance.GetAddressableSO(questData.CurrentQuestAddress) as Quest;
                if (_quest != null) LoadTutorialManager(_quest.tutorialForQuest);
            }
        }

        private string GetSavingPath(SavingPath tailPath)
        {
            var envPath = _gamePath + "/" + tailPath;
            return envPath;
        }
    }
}