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
        private IFactionController _factionController;
        private ObjectPool _creaturePool;
        private List<CreatureData> _creatureDatas;
        
        public void StartUpLoadData<T>(T data)
        {
            _creatureDatas = (List<CreatureData>)Convert.ChangeType(data, typeof(List<CreatureData>));
        }

        public void PlaceNewObject<T>(T data)
        {
            throw new NotImplementedException();
        }

        private void Start()
        {
            StartUpProcessor.Instance.OnInitiateObjects.AddListener(Init);
            _factionController = GetComponent<IFactionController>();
            _creaturePool = GetComponent<ObjectPool>();
        }

        private void Init()
        {
            foreach (var creatureData in _creatureDatas)
            {
                var creatureObj = _creaturePool.GetObject();
                creatureData.CreatureType = _factionController.GetFaction(); // adjust Faction to ensure it did not went wrong during customization
                StartUpProcessor.Instance.OnDomainRegister.Invoke(creatureObj, _factionController.GetFaction());

                if (creatureObj.TryGetComponent(out CreatureInGame creatureInGame))
                {
                    creatureInGame.gameObject.SetActive(true);
                    creatureInGame.Init(creatureData,_factionController);
                }
            }
            
            _factionController.Init();
        }
        
        public void Reset()
        {
            _creaturePool.ResetPool();
            _creatureDatas = new();
            _factionController.ResetData();
        }
    }
}
