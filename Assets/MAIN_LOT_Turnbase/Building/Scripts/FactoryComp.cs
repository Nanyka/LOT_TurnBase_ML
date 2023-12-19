using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class FactoryComp : MonoBehaviour, IInputExecutor, IStoreResource, IResearchDeliver
    {
        [SerializeField] private Vector3 assemblyPoint;
        [SerializeField] private TroopType troopType;
        [SerializeField] private string troopName;
        [SerializeField] private int troopCount;
        [SerializeField] private int costPerTroop;
        [SerializeField] private Transform testAssemblyPoint;

        private IBuildingDealer m_Building;
        private IEntityUIUpdate m_UIUpdater;
        private ICheckableObject m_CheckableObj;
        private List<Research> _researches = new();
        private int _curStorage;
        private int _spawnableAmount;
        private float _curWeight = 1f;
        private bool _isOnHolding;

        private void OnDisable()
        {
            ResetVariables();
        }

        private void ResetVariables()
        {
            _curWeight = 1f;
            _curStorage = 0;
            _spawnableAmount = 0;
            _isOnHolding = false;
        }

        private void Start()
        {
            m_Building = GetComponent<IBuildingDealer>();
            m_UIUpdater = GetComponent<IEntityUIUpdate>();
            m_CheckableObj = GetComponent<ICheckableObject>();
        }

        public void OnClick()
        {
            // Increase one priority for this factory and decrease one for the rest factories
            var factories = SavingSystemManager.Instance.GetStorageController().GetStorages();
            foreach (var factory in factories)
                factory.ReduceWeight(1);

            _curWeight = Mathf.Clamp(_curWeight + 10, 0, 100);
            m_UIUpdater.UpdateStorage(_curWeight);
        }

        public void OnHoldEnter()
        {
            _isOnHolding = true;
        }

        public void OnHolding(Vector3 position)
        {
            assemblyPoint = position;
            testAssemblyPoint.position = assemblyPoint;
        }

        public async void OnHoldCanCel()
        {
            if (_isOnHolding == false)
                return;

            _isOnHolding = false;
            if (_curStorage < costPerTroop)
                return;

            if (IsFullStock())
                m_Building.GetWorldStateChanger().ChangeState(1); // restart the factory after release a part of its storage

            _spawnableAmount = Mathf.FloorToInt(_curStorage * 1f / costPerTroop);
            for (int i = 0; i < _spawnableAmount; i++)
            {
                var troop = await SavingSystemManager.Instance.OnTrainACreature(troopName, transform.position, false);
                if (troop == null)
                    continue;

                if (troop.TryGetComponent(out ITroopAssembly character))
                    character.SetAssemblyPoint(assemblyPoint);

                if (troop.TryGetComponent(out ISpecialSkillReceiver attackReceiver))
                {
                    // Check if any storage research in the stock
                    // Plug the skill in the entity

                    foreach (var research in _researches)
                    {
                        switch (research.ResearchType)
                        {
                            case ResearchType.TROOP_TRANSFORM:
                                attackReceiver.EnablePowerBar(research.Magnitude);
                                break;
                        }
                    }
                }
            }

            _curStorage -= _spawnableAmount * costPerTroop;
            _spawnableAmount = 0;
            UpdateTroopAmountUI();
        }

        public void OnDoubleTaps()
        {
            Debug.Log($"Double taps on {name}");
        }

        public bool IsFullStock()
        {
            return _curStorage >= costPerTroop * troopCount || m_CheckableObj.IsCheckable() == false;
        }

        public void StoreResource(int amount)
        {
            if (IsFullStock())
                return;
            
            _curStorage = Mathf.Clamp(_curStorage + amount, 0, costPerTroop * troopCount);

            // Show amount of ready troop
            _spawnableAmount = Mathf.FloorToInt(_curStorage * 1f / costPerTroop);
            UpdateTroopAmountUI();

            // Remove one "Factory" state when the factory is full of stock
            if (IsFullStock())
            {
                _curStorage = costPerTroop * troopCount;
                m_Building.GetWorldStateChanger().ChangeState(-1); // decrease one unit of available factory
            }
        }

        private void UpdateTroopAmountUI()
        {
            if (_spawnableAmount > 0)
            {
                m_UIUpdater.UpdatePriceText(_spawnableAmount);
                m_UIUpdater.ShowPriceTag(true);
            }
            else
                m_UIUpdater.ShowPriceTag(false);

            MainUI.Instance.OnUpdateCurrencies.Invoke();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public float GetWeight()
        {
            return _curWeight;
        }

        public void ReduceWeight(int amount)
        {
            _curWeight -= amount;
            m_UIUpdater.UpdateStorage(Mathf.Clamp(_curWeight, 0, 100));
        }

        public int GetSpawnableAmount()
        {
            return _spawnableAmount;
        }

        public bool CheckTarget(string targetName)
        {
            return troopName.Equals(targetName);
        }

        public bool CheckTroopType(TroopType checkType)
        {
            return checkType == TroopType.NONE || checkType == troopType;
        }

        public void LoadResearch(Research research)
        {
            _researches.Add(research);
        }
    }

    public interface IStoreResource
    {
        public bool IsFullStock();
        public void StoreResource(int amount);
        public GameObject GetGameObject();
        public float GetWeight();
        public void ReduceWeight(int amount);
        public int GetSpawnableAmount();
    }

    public interface IResearchDeliver
    {
        public bool CheckTarget(string targetName);
        public bool CheckTroopType(TroopType checkType);
        public void LoadResearch(Research research);
    }
}