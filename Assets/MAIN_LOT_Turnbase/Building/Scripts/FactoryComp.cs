using System;
using UnityEngine;

namespace JumpeeIsland
{
    public class FactoryComp : MonoBehaviour, IInputExecutor, IStoreResource
    {
        [SerializeField] private Vector3 assemblyPoint;
        [SerializeField] private string troopName;
        [SerializeField] private int troopCount;
        [SerializeField] private int costPerTroop;

        [SerializeField] private int _curStorage;
        private BuildingEntity m_Building;

        private void Start()
        {
            m_Building = GetComponent<BuildingEntity>();
        }

        public void OnClick()
        {
            Debug.Log($"Click on {name}");
        }

        public void OnHoldEnter()
        {
            Debug.Log($"Hold on {name}");
        }

        public void OnHolding(Vector3 position)
        {
            assemblyPoint = position;
        }

        public async void OnHoldCanCel()
        {
            for (int i = 0; i < troopCount; i++)
            {
                var troop = await SavingSystemManager.Instance.OnTrainACreature(troopName, transform.position, false);
                if (troop == null)
                    continue;

                if (troop.TryGetComponent(out CharacterEntity character))
                    character.SetAssemblyPoint(assemblyPoint);
            }
            troopCount = 0;
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
            _curStorage += amount;
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
    }
}