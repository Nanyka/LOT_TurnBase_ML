using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectableObjectLoader : MonoBehaviour, ILoadData
    {
        [SerializeField] private ObjectPool _collectablePool;
        private List<CollectableData> _collectableDatas;
        private CollectableController _collectableController;

        private void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _collectableController = GetComponent<CollectableController>();
        }

        public void StartUpLoadData<T>(T data)
        {
            _collectableDatas = (List<CollectableData>)Convert.ChangeType(data, typeof(List<CollectableData>));
        }

        private void Init()
        {
            if (_collectableDatas == null)
                return;

            foreach (var collectable in _collectableDatas)
            {
                SpawnCollectableObject(collectable);
            }

            _collectableController.Init();
        }

        public void PlaceNewObject<T>(T data)
        {
            var collectableData = (CollectableData)Convert.ChangeType(data, typeof(CollectableData));
            SpawnCollectableObject(collectableData);
        }

        private void SpawnCollectableObject(CollectableData collectableData)
        {
            var collectableObj = _collectablePool.GetObject(collectableData.EntityName);
            if (collectableObj == null)
                return;

            collectableData.CreatureType = FactionType.Neutral; // assign Faction
            // GameFlowManager.Instance.OnDomainRegister.Invoke(resourceObj, resourceData.CreatureType);

            if (!collectableObj.TryGetComponent(out CollectableInGame collectableInGame)) return;
            collectableInGame.gameObject.SetActive(true);
            collectableInGame.Init(collectableData, _collectableController);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}