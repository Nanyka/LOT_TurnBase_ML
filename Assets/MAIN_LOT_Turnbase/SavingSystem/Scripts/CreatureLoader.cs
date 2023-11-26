using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    [RequireComponent(typeof(IFactionController))]
    [RequireComponent(typeof(ObjectPool))]
    public class CreatureLoader : MonoBehaviour, ILoadData
    {
        protected IFactionController _factionController;
        protected ObjectPool _creaturePool;
        protected List<CreatureData> _creatureDatas;

        public void StartUpLoadData<T>(T data)
        {
            _creatureDatas = (List<CreatureData>)Convert.ChangeType(data, typeof(List<CreatureData>));
        }

        protected virtual void Start()
        {
            GameFlowManager.Instance.OnInitiateObjects.AddListener(Init);
            _factionController = GetComponent<IFactionController>();
            _creaturePool = GetComponent<ObjectPool>();
        }

        protected virtual void Init()
        {
            foreach (var creatureData in _creatureDatas)
                TrainANewCreature(creatureData);

            _factionController.Init();
        }

        public virtual GameObject PlaceNewObject<T>(T data)
        {
            var creatureData = (CreatureData)Convert.ChangeType(data, typeof(CreatureData));
            return TrainANewCreature(creatureData);
        }

        protected virtual GameObject TrainANewCreature(CreatureData creatureData)
        {
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
}