using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [RequireComponent(typeof(IFactionController))]
    [RequireComponent(typeof(ObjectPool))]
    public class CreatureLoader : MonoBehaviour, ICreatureLoader
    {
        protected IFactionController _factionController;
        private ObjectPool _creaturePool;
        protected List<CreatureData> _creatureDatas;
        
        protected virtual void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _factionController = GetComponent<IFactionController>();
            _creaturePool = GetComponent<ObjectPool>();
        }

        public virtual void Init()
        {
            foreach (var creatureData in _creatureDatas)
                TrainANewCreature(creatureData);

            _factionController.Init();
        }
        
        public void StartUpLoadData(List<CreatureData> data)
        {
            _creatureDatas = data;
        }

        public virtual GameObject PlaceNewObject(CreatureData data)
        {
            return TrainANewCreature(data);
        }

        protected virtual GameObject TrainANewCreature(CreatureData creatureData)
        {
            creatureData.EntityType =
                creatureData.FactionType == FactionType.Player ? EntityType.PLAYER : EntityType.ENEMY;
            
            var creatureObj = _creaturePool.GetObject(creatureData.EntityName);
            if (creatureObj == null)
                return null;

            creatureData.FactionType = _factionController.GetFaction(); // assign Faction
            GameFlowManager.Instance.OnDomainRegister.Invoke(creatureObj, _factionController.GetFaction());

            if (creatureObj.TryGetComponent(out CreatureInGame creatureInGame))
            {
                creatureInGame.gameObject.SetActive(true);
                creatureInGame.Init(creatureData, _factionController);
            }

            return creatureObj;
        }

        public void Reset()
        {
            _creaturePool.ResetPool();
            _creatureDatas = new();
            _factionController.ResetData();
        }
    }

    public interface ICreatureLoader
    {
        public void Init();
        public void StartUpLoadData(List<CreatureData> data);
        public GameObject PlaceNewObject(CreatureData data);
        public void Reset();
    }
}