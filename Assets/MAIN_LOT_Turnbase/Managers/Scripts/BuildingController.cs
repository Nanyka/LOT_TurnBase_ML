using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingController : MonoBehaviour
    {
        private List<BuildingInGame> m_buildings = new();
        private EnvironmentManager m_Environment;
        private Camera _camera;
        private int _layerMask = 1 << 9;

        public void Init()
        {
            _camera = Camera.main;
            m_Environment = GameFlowManager.Instance.GetEnvManager();
            m_Environment.OnChangeFaction.AddListener(BuildingInTurn);
        }

        private void BuildingInTurn()
        {
            foreach (var building in m_buildings)
            {
                if (m_Environment.GetCurrFaction() != building.GetEntity().GetFaction())
                    continue;
                
                building.DurationDeduct(m_Environment.GetCurrFaction());

                if (GameFlowManager.Instance.GameMode == GameMode.ECONOMY)
                    return;
                building.AskForAttack();
            }
        }

        public void AddBuildingToList(BuildingInGame building)
        {
            m_buildings.Add(building);
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (MainUI.Instance.IsInteractable == false || PointingChecker.IsPointerOverUIObject())
                    return;
                
                var moveRay = _camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(moveRay, out var moveHit, 100f, _layerMask))
                    return;

                var pos = moveHit.point;
                SelectUnit(new Vector3(Mathf.RoundToInt(pos.x), 0f, Mathf.RoundToInt(pos.z)));
            }
        }

        private void SelectUnit(Vector3 unitPos)
        {
            var getUnitAtPos = GetUnitByPos(unitPos);
            if (getUnitAtPos == null) return;

            HighlightSelectedUnit(getUnitAtPos);
        }

        private BuildingInGame GetUnitByPos(Vector3 unitPos)
        {
            return m_buildings.Find(x => Vector3.Distance(x.transform.position, unitPos) < Mathf.Epsilon);
        }

        private void HighlightSelectedUnit(BuildingInGame buildingInGame)
        {
            MainUI.Instance.OnShowInfo.Invoke(buildingInGame);
            // MainUI.Instance.OnInteractBuildingMenu.Invoke(getUnitAtPos);
        }

        public void StoreRewardAtBuildings(string currencyId, int amount)
        {
            if (currencyId.Equals("GOLD") || currencyId.Equals("GEM") ||  currencyId.Equals("MOVE"))
                return;
            
            // Check if enough storage space
            int availableSpace = 0;
            List<BuildingEntity> selectedBuildings = new List<BuildingEntity>();
            if (Enum.TryParse(currencyId, out CurrencyType currency))
                foreach (var t in m_buildings)
                    availableSpace += t.GetStoreSpace(currency, ref selectedBuildings);

            if (amount > availableSpace)
                MainUI.Instance.OnConversationUI.Invoke($"Lack of {currencyId} STORAGE. Current storage is {availableSpace} and need for {amount}",true);
            
            amount = amount > availableSpace ? availableSpace : amount;

            if (amount == 0 || selectedBuildings.Count == 0)
                return;
            
            GeneralAlgorithm.Shuffle(selectedBuildings); // Shuffle buildings to ensure random selection

            // Stock currency to building and gain exp
            foreach (var building in selectedBuildings)
            {
                if (amount <= 0)
                    break;
                var storeAmount = building.GetStorageSpace(currency);
                storeAmount = storeAmount > amount ? amount : storeAmount;
                building.StoreCurrency(storeAmount);
                SavingSystemManager.Instance.IncrementLocalCurrency(currencyId, storeAmount);
                amount -= storeAmount;
            }
        }

        public void DeductCurrencyFromBuildings(string currencyId, int amount)
        {
            Debug.Log($"Deduct {currencyId} from buildings");
            if (currencyId.Equals("GOLD") || currencyId.Equals("GEM") ||  currencyId.Equals("MOVE"))
                return;
            
            // Check if enough storage space
            int currentStorage = 0;
            List<BuildingEntity> selectedBuildings = new List<BuildingEntity>();
            if (Enum.TryParse(currencyId, out CurrencyType currency))
                foreach (var t in m_buildings)
                    currentStorage += t.GetCurrenStorage(currency, ref selectedBuildings);
            
            if (amount > currentStorage)
            {
                Debug.Log($"Lack of {currencyId}");
                return;
            } 
            
            GeneralAlgorithm.Shuffle(selectedBuildings); // Shuffle buildings to ensure random selection

            // Stock currency to building and gain exp
            foreach (var building in selectedBuildings)
            {
                if (amount <= 0)
                    break;
                var deductedAmount = building.GetCurrentStorage(currency);
                deductedAmount = deductedAmount > amount ? amount : deductedAmount;
                building.DeductCurrency(deductedAmount);
                // SavingSystemManager.Instance.DeductCurrency(currencyId, deductedAmount);
                amount -= deductedAmount;
            }
        }

        public void RemoveBuilding(BuildingInGame building)
        {
            m_Environment.RemoveObject(building.gameObject, building.GetEntity().GetFaction());
            m_buildings.Remove(building);
        }
    }
}