using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectableObjectLoader : MonoBehaviour, ILoadData
    {
        [SerializeField] private ObjectPool _collectablePool;
        private List<CollectableData> _collectableData = new();
        private CollectableController _collectableController;

        private void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _collectableController = GetComponent<CollectableController>();
        }

        public void StartUpLoadData<T>(T data)
        {
            _collectableData = (List<CollectableData>)Convert.ChangeType(data, typeof(List<CollectableData>));
        }

        private void Init()
        {
            if (_collectableData == null)
                return;

            foreach (var collectable in _collectableData)
            {
                SpawnCollectableObject(collectable);
            }

            _collectableController.Init();
        }

        public GameObject PlaceNewObject<T>(T data)
        {
            var collectableData = (CollectableData)Convert.ChangeType(data, typeof(CollectableData));
            return SpawnCollectableObject(collectableData);
        }

        private GameObject SpawnCollectableObject(CollectableData collectableData)
        {
            var collectableObj = _collectablePool.GetObject(collectableData.EntityName);
            if (collectableObj == null)
                return null;

            collectableData.FactionType = FactionType.Neutral; // assign Faction
            // GameFlowManager.Instance.OnDomainRegister.Invoke(resourceObj, resourceData.CreatureType);

            if (!collectableObj.TryGetComponent(out CollectableInGame collectableInGame)) return null;
            collectableInGame.gameObject.SetActive(true);
            collectableInGame.Init(collectableData, _collectableController);
            return collectableObj;
        }

        public void Reset()
        {
            _collectablePool.ResetPool();
            _collectableData = new List<CollectableData>();
        }
    }
}