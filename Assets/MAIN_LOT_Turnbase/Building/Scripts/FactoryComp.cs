using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace JumpeeIsland
{
    public class FactoryComp : MonoBehaviour, IInputExecutor, IStoreResource
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
        private IHealthComp m_HealthComp;
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

        private void Awake()
        {
            m_Building = GetComponent<IBuildingDealer>();
            m_UIUpdater = GetComponent<IEntityUIUpdate>();
            m_CheckableObj = GetComponent<ICheckableObject>();
            m_HealthComp = GetComponent<IHealthComp>();
        }

        private void Start()
        {
            assemblyPoint = transform.position;
            testAssemblyPoint.position = assemblyPoint;
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
            if (_isOnHolding == false || IsFullStock() == false)
                return;

            _isOnHolding = false;
            // if (_curStorage < costPerTroop)
            //     return;

            // if (IsFullStock())
            //     m_Building.GetWorldStateChanger().ChangeState(1); // restart the factory after release a part of its storage

            await SpawnTroop();
        }

        public async void OnDoubleTaps()
        {
            Debug.Log($"Double taps on {name}");
            
            if (IsFullStock() == false)
                return;

            await SpawnTroop();
        }

        private async Task SpawnTroop()
        {
            _spawnableAmount = Mathf.FloorToInt(_curStorage * 1f / costPerTroop);
            for (int i = 0; i < _spawnableAmount; i++)
            {
                var troop = await SavingSystemManager.Instance.OnTrainACreature(troopName, transform.position, false);
                if (troop == null)
                    continue;

                if (troop.TryGetComponent(out ITroopAssembly character))
                    character.SetAssemblyPoint(assemblyPoint);

                if (troop.TryGetComponent(out IComboReceiver attackReceiver))
                {
                    // Retrieve all researches
                    // Check if any research is relevant to the training troop
                    // Apply the feasible research on the troop

                    var researches = SavingSystemManager.Instance.GetResearchTopics();
                    researches = researches.Where(t => t.IsUnlocked);
                    foreach (var research in researches)
                    {
                        switch (research.ResearchType)
                        {
                            case ResearchType.TROOP_TRANSFORM:
                            {
                                if (troopName.Equals(research.Target))
                                    attackReceiver.EnablePowerBar(research.Magnitude);

                                break;
                            }
                            case ResearchType.TROOP_STATS:
                            {
                                if (research.TroopType == TroopType.NONE || research.TroopType == troopType)
                                    Debug.Log("Do some stats changing");

                                break;
                            }
                        }
                    }
                }
            }

            _curStorage -= _spawnableAmount * costPerTroop;
            _spawnableAmount = 0;
            UpdateTroopAmountUI();

            // Destroy the factory after completed the training
            m_HealthComp.HideTheEntity();
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
            if (IsFullStock())
            {
                m_UIUpdater.UpdatePriceText(_spawnableAmount);
                m_UIUpdater.ShowPriceTag(true);
            }
            else
                m_UIUpdater.ShowPriceTag(false);
        
            // MainUI.Instance.OnUpdateCurrencies.Invoke();
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
}