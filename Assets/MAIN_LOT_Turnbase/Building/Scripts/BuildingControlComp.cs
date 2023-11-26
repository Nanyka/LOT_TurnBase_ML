using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingControlComp : MonoBehaviour, IInputExecutor
    {
        [SerializeField] private Vector3 assemblyPoint;
        [SerializeField] private string troopName;
        [SerializeField] private int troopCount;
        
        public void OnClick()
        {
            Debug.Log($"Click on {name}");
        }

        public void OnHoldEnter()
        {
            Debug.Log($"Hold on {name}");
        }

        public void OnHolding(Vector3 position)
        {
            assemblyPoint = position;
        }

        public async void OnHoldCanCel()
        {
            for (int i = 0; i < troopCount; i++)
            {
                var troop = await SavingSystemManager.Instance.OnTrainACreature(troopName, transform.position, false);
                if (troop == null)
                    continue;

                if (troop.TryGetComponent(out CharacterEntity character))
                    character.SetAssemblyPoint(assemblyPoint);
            }
            troopCount = 0;
        }

        public void OnDoubleTaps()
        {
            Debug.Log($"Double taps on {name}");
        }
    }
}