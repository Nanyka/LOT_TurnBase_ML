using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    [RequireComponent(typeof(IFactionController))]
    public class CreatureManager : MonoBehaviour, IStartUpLoadData
    {
        [SerializeField] private ObjectPool _creaturePool;
        
        private IFactionController _playerFaction;
        private List<CreatureData> _creatureDatas;
        
        public void StartUpLoadData<T>(T data)
        {
            _creatureDatas = (List<CreatureData>)Convert.ChangeType(data, typeof(List<CreatureData>));
        }
        
        private void Start()
        {
            StartUpProcessor.Instance.OnInitiateObjects.AddListener(Init);
            _playerFaction = GetComponent<IFactionController>();
        }

        private void Init()
        {
            foreach (var creatureData in _creatureDatas)
            {
                var creatureObj = _creaturePool.GetObject();
                StartUpProcessor.Instance.OnDomainRegister.Invoke(creatureObj, _playerFaction.GetFaction());

                if (creatureObj.TryGetComponent(out CreatureInGame creatureInGame))
                {
                    creatureInGame.gameObject.SetActive(true);
                    creatureInGame.Init(creatureData,_playerFaction);
                }
            }
            
            _playerFaction.Init();
        }
    }
}
