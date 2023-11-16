using UnityEngine;

namespace JumpeeIsland
{
    public class BuildingControlComp : MonoBehaviour, IInputExecutor
    {
        [SerializeField] private Transform assemblyPoint;
        [SerializeField] private GameObject troop;
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
            assemblyPoint.position = position;
        }

        public void OnHoldCanCel()
        {
            for (int i = 0; i < troopCount; i++)
            {
                var spawnTroop = Instantiate(troop, transform.position, Quaternion.identity);
                if (spawnTroop.TryGetComponent(out CharacterEntity character))
                {
                    character.SetAssemblyPoint(assemblyPoint.position);
                }
            }
            troopCount = 0;
        }

        public void OnDoubleTaps()
        {
            Debug.Log($"Double taps on {name}");
        }
    }
}