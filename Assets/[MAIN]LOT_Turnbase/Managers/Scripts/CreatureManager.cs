using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LOT_Turnbase
{
    [RequireComponent(typeof(FactionController))]
    public class CreatureManager : MonoBehaviour, IStartUpLoadData
    {
        [SerializeField] private ObjectPool _creaturePool;
        [SerializeField] private FactionController _faction;
        private List<CreatureData> _creatureDatas;
        
        public void StartUpLoadData<T>(T data)
        {
            _creatureDatas = (List<CreatureData>)Convert.ChangeType(data, typeof(List<CreatureData>));
        }
        
        private void Start()
        {
            StartUpProcessor.Instance.OnInitiateObjects.AddListener(Init);
        }

        private void Init()
        {
            foreach (var creatureData in _creatureDatas)
            {
                var creatureObj = _creaturePool.GetObject();
                StartUpProcessor.Instance.OnDomainRegister.Invoke(creatureObj, _faction.GetFaction());

                if (creatureObj.TryGetComponent(out CreatureInGame creatureInGame))
                {
                    creatureInGame.gameObject.SetActive(true);
                    creatureInGame.Init(creatureData,_faction);
                }
            }
        }
    }
}
