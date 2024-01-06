using UnityEngine;

namespace JumpeeIsland
{
    // Transform means destroy this character and spawn a new one that may be the next level of this character
    public class AoeCharacterTransformComp : MonoBehaviour
    {
        [SerializeField] private string _nameOfTranformer;
        [SerializeField] private int _levelOfTranformer;
        
        public void ExecuteTransform()
        {
            // Call to HpComp to destroy this character after a certain amount of second
            // Spawn the new character
            
            GetComponent<IHealthComp>().HideTheEntity();
            
            CreatureData transformerData = new CreatureData
            {
                EntityName = _nameOfTranformer,
                CurrentLevel = _levelOfTranformer,
                Position = transform.position
            };

            SavingSystemManager.Instance.OnSpawnMovableEntity(_nameOfTranformer, transform.position);
        }
    }
}