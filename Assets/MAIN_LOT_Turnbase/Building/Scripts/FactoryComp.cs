using System;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField] private Slider priorityBar;

        private int _curStorage;
        private IBuildingDealer m_Building;
        private IEntityUIUpdate m_UIUpdater;
        private Research m_Research;
        private float _curWeight = 1f;
        private bool _isOnHolding;

        private void Start()
        {
            m_Building = GetComponent<IBuildingDealer>();
            m_UIUpdater = GetComponent<IEntityUIUpdate>();
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

            if (_curStorage >= costPerTroop * troopCount)
                m_Building.GetWorldStateChanger()
                    .ChangeState(1); // restart the factory after release a part of its storage

            var spawnAmount = Mathf.FloorToInt(_curStorage * 1f / costPerTroop);
            for (int i = 0; i < spawnAmount; i++)
            {
                var troop = await SavingSystemManager.Instance.OnTrainACreature(troopName, transform.position, false);
                if (troop == null)
                    continue;

                if (troop.TryGetComponent(out ITroopAssembly character))
                    character.SetAssemblyPoint(assemblyPoint);

                if (troop.TryGetComponent(out ISpecialAttackReceiver attackReceiver))
                {
                    // Check if any storage research in the stock
                    // Plug the skill in the entity

                    if (m_Research != null)
                        attackReceiver.EnablePowerBar(m_Research.Magnitude);
                }
            }

            _curStorage -= spawnAmount * costPerTroop;
        }

        public void OnDoubleTaps()
        {
            Debug.Log($"Double taps on {name}");
        }

        public bool IsFullStock()
        {
            return _curStorage >= costPerTroop * troopCount;
        }

        public void StoreResource(int amount)
        {
            _curStorage = Mathf.Clamp(_curStorage + amount, 0, costPerTroop * troopCount);

            // Show amount of ready troop
            var spawnAmount = Mathf.FloorToInt(_curStorage * 1f / costPerTroop);
            if (spawnAmount > 0)
            {
                m_UIUpdater.UpdatePriceText(spawnAmount);
                m_UIUpdater.ShowPriceTag(true);
            }
            else
                m_UIUpdater.ShowPriceTag(false);

            // Remove one "Factory" state when the factory is full of stock
            if (_curStorage >= costPerTroop * troopCount)
            {
                _curStorage = costPerTroop * troopCount;
                m_Building.GetWorldStateChanger().ChangeState(-1); // decrease one unit of available factory
            }
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
            m_UIUpdater.UpdateStorage(Mathf.Clamp(_curWeight,0,100));
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
            m_Research = research;
        }
    }

    public interface IStoreResource
    {
        public bool IsFullStock();
        public void StoreResource(int amount);
        public GameObject GetGameObject();
        public float GetWeight();
        public void ReduceWeight(int amount);
    }

    public interface IResearchDeliver
    {
        public bool CheckTarget(string targetName);
        public bool CheckTroopType(TroopType checkType);
        public void LoadResearch(Research research);
    }
}