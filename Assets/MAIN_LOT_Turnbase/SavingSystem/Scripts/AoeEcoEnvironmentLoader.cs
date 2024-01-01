using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeEcoEnvironmentLoad : MonoBehaviour, IEnvironmentLoad
    {
        [SerializeField] protected GameObject buildingLoader;
        [SerializeField] private GameObject playerLoader;

        private EnvironmentData _environmentData;
        private IBuildingLoader m_BuildingLoader;
        private ICreatureLoader m_GuardianLoader;

        public void Init()
        {
            m_BuildingLoader = buildingLoader.GetComponent<IBuildingLoader>();
            m_GuardianLoader = playerLoader.GetComponent<ICreatureLoader>();
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            Debug.Log("Load data into managers...");
            ExecuteEnvData();
        }

        private void RemoveDestroyedEntity(IRemoveEntity removeInterface)
        {
            var entityData = removeInterface.GetEntityData();

            switch (entityData.EntityType)
            {
                case EntityType.BUILDING:
                {
                    GetData().BuildingData.Remove(entityData as BuildingData);
                    break;
                }
                case EntityType.PLAYER:
                {
                    GetData().PlayerData.Remove(entityData as CreatureData);
                    break;
                }
                case EntityType.ENEMY:
                {
                    GetData().EnemyData.Remove(entityData as CreatureData);
                    break;
                }
                case EntityType.RESOURCE:
                {
                    GetData().ResourceData.Remove(entityData as ResourceData);
                    break;
                }
                case EntityType.COLLECTABLE:
                {
                    GetData().CollectableData.Remove(entityData as CollectableData);
                    break;
                }
            }
        }

        private void ExecuteEnvData()
        {
            m_BuildingLoader.StartUpLoadData(_environmentData.BuildingData);
            m_GuardianLoader.StartUpLoadData(new List<CreatureData>());
            SavingSystemManager.Instance.OnSyncEnvData();
            GameFlowManager.Instance.OnInitiateObjects.Invoke();
        }

        public void SetData(EnvironmentData environmentData)
        {
            if (environmentData == null)
                return;
            _environmentData = environmentData;
        }

        public EnvironmentData GetData()
        {
            return _environmentData;
        }

        public EnvironmentData GetDataForSave()
        {
            _environmentData.PlayerData.Clear();
            return _environmentData;
        }

        public void ResetData()
        {
            Debug.Log("Remove all environment to reset...");
            m_BuildingLoader.Reset();
        }

        public void SpawnResource(ResourceData resourceData)
        {
            throw new System.NotImplementedException();
        }

        public void SpawnCollectable(CollectableData collectableData)
        {
            throw new System.NotImplementedException();
        }

        public void PlaceABuilding(BuildingData buildingData)
        {
            _environmentData.AddBuildingData(buildingData);
            m_BuildingLoader.PlaceNewObject(buildingData);
        }

        public GameObject TrainACreature(CreatureData creatureData)
        {
            _environmentData.AddPlayerData(creatureData);
            return m_GuardianLoader.PlaceNewObject(creatureData);
        }

        public GameObject SpawnAnEnemy(CreatureData creatureData)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<GameObject> GetBuildings(FactionType faction)
        {
            return m_BuildingLoader.GetBuildings();
        }

        public IEnumerable<GameObject> GetResources()
        {
            throw new System.NotImplementedException();
        }
    }
}