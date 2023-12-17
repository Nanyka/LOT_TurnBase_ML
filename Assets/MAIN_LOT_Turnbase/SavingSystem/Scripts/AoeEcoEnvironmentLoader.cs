using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeEcoEnvironmentLoader : MonoBehaviour, IEnvironmentLoader
    {
        [SerializeField] protected BuildingLoader buildingLoader;
        [SerializeField] private CreatureLoader playerLoader;
        [SerializeField] protected EnvironmentData _environmentData;

        public void Init()
        {
            SavingSystemManager.Instance.OnRemoveEntityData.AddListener(RemoveDestroyedEntity);
            Debug.Log("Load data into managers...");
            ExecuteEnvData();
        }
        
        protected void RemoveDestroyedEntity(IRemoveEntity removeInterface)
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
            buildingLoader.StartUpLoadData(_environmentData.BuildingData);
            playerLoader.StartUpLoadData(new List<CreatureData>());
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
            _environmentData.RemoveZeroHpPlayerCreatures();
            return _environmentData;
        }

        public void ResetData()
        {
            Debug.Log("Remove all environment to reset...");
            buildingLoader.Reset();
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
            buildingLoader.PlaceNewObject(buildingData);
        }

        public GameObject TrainACreature(CreatureData creatureData)
        {
            _environmentData.AddPlayerData(creatureData);
            return playerLoader.PlaceNewObject(creatureData);
        }

        public GameObject SpawnAnEnemy(CreatureData creatureData)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<GameObject> GetBuildings(FactionType faction)
        {
            return buildingLoader.GetBuildings();
        }

        public IEnumerable<GameObject> GetResources()
        {
            throw new System.NotImplementedException();
        }
    }
}