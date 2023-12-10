using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingInGame : MonoBehaviour, IShowInfo, IConfirmFunction, IRemoveEntity, IGetEntityInfo,
        IAttackResponse, IBuildingDealer
    {
        [SerializeField] private BuildingEntity m_Entity;

        private IBuildingController _buildingController;

        public void Init(BuildingData buildingData, IBuildingController buildingController)
        {
            m_Entity.Init(buildingData);
            transform.position = buildingData.Position;

            _buildingController = buildingController;
            _buildingController.AddBuildingToList(this);
        }

        public IChangeWorldState GetWorldStateChanger()
        {
            throw new System.NotImplementedException();
        }

        public virtual void OnEnable()
        {
            m_Entity.OnUnitDie.AddListener(DestroyBuilding);
        }

        private void OnDisable()
        {
            m_Entity.OnUnitDie.RemoveListener(DestroyBuilding);
        }

        public void DurationDeduct(FactionType currentFaction)
        {
            if (currentFaction != m_Entity.GetFaction())
                return;

            m_Entity.DurationDeduct();
        }

        public int GetStoreSpace(CurrencyType currency, ref List<BuildingEntity> selectedBuildings)
        {
            return m_Entity.GetStorageSpace(currency, ref selectedBuildings);
        }

        public int GetCurrenStorage(CurrencyType currency, ref List<BuildingEntity> selectedBuildings)
        {
            return m_Entity.GetCurrentStorage(currency, ref selectedBuildings);
        }

        public (Entity, int) ShowInfo()
        {
            return (m_Entity, 0);
        }

        public void ClickYes()
        {
            if (SavingSystemManager.Instance.GetEnvironmentData().BuildingData
                    .Count(t => t.BuildingType == m_Entity.GetBuildingType()) <= 1)
            {
                MainUI.Instance.OnConversationUI.Invoke($"Cannot sell the only one {m_Entity.GetBuildingType()} left",true);
                return;
            }

            DestroyBuilding(m_Entity);
            SellBuilding(SavingSystemManager.Instance.GetEnvironmentData());
        }

        private void DestroyBuilding(IAttackRelated killedByEntity)
        {
            // just contribute resource when it is killed by player faction as selling out this building
            if (killedByEntity.GetFaction() == FactionType.Player && GameFlowManager.Instance.GameMode != GameMode.REPLAY)
                SavingSystemManager.Instance.GrantCurrency(CurrencyType.GOLD.ToString(),
                    m_Entity.CalculateSellingPrice());

            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);

            StartCoroutine(DestroyVisual());
        }

        private IEnumerator DestroyVisual()
        {
            // VFX
            yield return new WaitForSeconds(1f);
            Debug.Log("Remove building");
            _buildingController.RemoveBuilding(this);
            MainUI.Instance.OnUpdateCurrencies.Invoke();
            gameObject.SetActive(false);
        }

        // public void Remove(IEnvironmentLoader envLoader)
        // {
        //     envLoader.GetData().BuildingData.Remove((BuildingData)m_Entity.GetData());
        // }

        public GameObject GetRemovedObject()
        {
            return gameObject;
        }

        public EntityData GetEntityData()
        {
            return m_Entity.GetData();
        }

        private void SellBuilding(EnvironmentData environmentData)
        {
            environmentData.BuildingData.Remove((BuildingData)m_Entity.GetData());
        }

        public void AskForAttack()
        {
            if (m_Entity.GetBuildingType() == BuildingType.TOWER)
            {
                // Record building action in BATTLE mode
                if (GameFlowManager.Instance.GameMode == GameMode.BATTLE)
                {
                    var recordAction = new RecordAction
                    {
                        Action = 0,
                        // AtSecond = CountDownClock.GetBattleTime(),
                        AtPos = GetPosition(),
                        EntityType = EntityType.BUILDING
                    };
            
                    SavingSystemManager.Instance.OnRecordAction.Invoke(recordAction);
                }
                
                m_Entity.AttackSetup(this, this);
            }
        }

        public (Vector3 midPos, Vector3 direction, int jumpStep, FactionType faction) GetCurrentState()
        {
            return (transform.position, Vector3.zero, 1, m_Entity.GetFaction());
        }

        public void AttackResponse()
        {
            // Debug.Log("Building finished an attack");
        }

        #region GET
        
        public Entity GetEntity()
        {
            return m_Entity;
        }
        
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Vector3 GetPosition()
        {
            return m_Entity.GetData().Position;
        }

        #endregion
    }
}