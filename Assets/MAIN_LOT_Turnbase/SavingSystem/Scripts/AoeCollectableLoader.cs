using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeCollectableLoader : MonoBehaviour, ICollectableLoader
    {
        [SerializeField] private ObjectPool _collectablePool;
        private List<CollectableData> _collectableData = new();
        private CollectableController _collectableController;
        
        private void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _collectableController = GetComponent<CollectableController>();
        }
        
        private void Init()
        {
            if (_collectableData == null)
                return;

            foreach (var collectable in _collectableData)
                SpawnCollectableObject(collectable);
        }
        
        public void StartUpLoadData(List<CollectableData> data)
        {
            _collectableData = data;
        }

        public GameObject PlaceNewObject(CollectableData data)
        {
            return SpawnCollectableObject(data);
        }
        
        private GameObject SpawnCollectableObject(CollectableData collectableData)
        {
            collectableData.EntityType = EntityType.COLLECTABLE;
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