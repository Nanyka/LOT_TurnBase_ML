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
            m_Environment.OnChangeFaction.AddListener(DurationDeduct);
        }

        private void DurationDeduct()
        {
            foreach (var building in m_buildings)
                building.DurationDeduct(m_Environment.GetCurrFaction());
        }

        public void AddBuildingToList(BuildingInGame building)
        {
            m_buildings.Add(building);
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (MainUI.Instance.IsInteractable == false)
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

        private void HighlightSelectedUnit(BuildingInGame getUnitAtPos)
        {
            MainUI.Instance.OnShowInfo.Invoke(getUnitAtPos);
            MainUI.Instance.OnSellBuildingMenu.Invoke(getUnitAtPos);
        }

        public void StoreRewardToBuildings(string currencyId, int amount)
        {
            // Check if enough storage space
            int currentStorage = 0;
            Queue<BuildingEntity> selectedBuildings = new Queue<BuildingEntity>();
            if (Enum.TryParse(currencyId, out CurrencyType currency))
                foreach (var t in m_buildings)
                    currentStorage += t.GetStoreSpace(currency, ref selectedBuildings);

            amount = amount > currentStorage ? currentStorage : amount;

            if (amount == 0 || selectedBuildings.Count == 0)
                return;

            // Stock currency to building and grain exp
            while (amount > 0)
            {
                var building = selectedBuildings.Dequeue();
                var storeAmount = building.GetStorageSpace(currency);
                storeAmount = storeAmount > amount ? amount : storeAmount;
                building.StoreCurrency(storeAmount);
                amount -= storeAmount;
            }
        }

        public int GetStorageSpace(string currencyId)
        {
            int storageSpace = 0;
            Queue<BuildingEntity> selectedBuildings = new Queue<BuildingEntity>();
            if (Enum.TryParse(currencyId, out CurrencyType currency))
                foreach (var t in m_buildings)
                    storageSpace += t.GetStoreSpace(currency, ref selectedBuildings);
            return storageSpace;
        }

        public void RemoveBuilding(BuildingInGame building)
        {
            m_Environment.RemoveObject(building.gameObject, FactionType.Neutral);
            m_buildings.Remove(building);
        }
    }
}