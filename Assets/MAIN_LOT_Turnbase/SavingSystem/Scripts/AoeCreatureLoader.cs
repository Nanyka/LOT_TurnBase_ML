using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeCreatureLoader : MonoBehaviour, ICreatureLoader
    {
        private IFactionController _factionController;
        private ObjectPool _creaturePool;
        protected List<CreatureData> _creatureDatas;

        public void StartUpLoadData(List<CreatureData> data)
        {
            _creatureDatas = data;
        }

        protected virtual void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _factionController = GetComponent<IFactionController>();
            _creaturePool = GetComponent<ObjectPool>();
        }

        public void Init()
        {
            _factionController.Init();
        }

        public virtual GameObject PlaceNewObject(CreatureData data)
        {
            return TrainANewCreature(data);
        }

        private GameObject TrainANewCreature(CreatureData creatureData)
        {
            creatureData.EntityType =
                creatureData.FactionType == FactionType.Player ? EntityType.PLAYER : EntityType.ENEMY;
            var creatureObj = _creaturePool.GetObject(creatureData.EntityName);
            if (creatureObj == null)
                return null;

            creatureData.FactionType = _factionController.GetFaction(); // assign Faction
            GameFlowManager.Instance.OnDomainRegister.Invoke(creatureObj, _factionController.GetFaction());
            
            if (creatureObj.TryGetComponent(out CharacterEntity creatureInGame))
            {
                creatureInGame.gameObject.SetActive(true);
                creatureInGame.Init(creatureData);
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
}