using UnityEngine;

namespace JumpeeIsland
{
    // Transform means destroy this character and spawn a new one that may be the next level of this character
    public class AoeCharacterTransformComp : MonoBehaviour
    {
        [SerializeField] private CreatureData _transformerData;
        
        public void ExecuteTransform()
        {
            // Call to HpComp to destroy this character after a certain amount of second
            // Spawn the new character
            
            GetComponent<IHealthComp>().HideTheEntity();
            _transformerData.Position = transform.position;
            SavingSystemManager.Instance.OnSpawnMovableEntity(_transformerData);
        }
    }
}