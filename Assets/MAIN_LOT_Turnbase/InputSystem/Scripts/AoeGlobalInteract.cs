using UnityEngine;

namespace JumpeeIsland
{
    public class AoeGlobalInteract : MonoBehaviour, IGlobalInteract
    {
        [SerializeField] private float _castRange;
        [SerializeField] private LayerMask _castLayer;
        
        public void OnDoubleTap(Vector3 tapPos)
        {
            GameFlowManager.Instance.AskGlobalVfx(GlobalVfxType.RADAR, tapPos);

            Collider[] hitColliders = new Collider[10];
            int numColliders = Physics.OverlapSphereNonAlloc(tapPos, _castRange, hitColliders, _castLayer);

            if (numColliders > 0)
            {
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider == null)
                        continue;
                    
                    // Ask the unit move to tapPos
                    if (hitCollider.gameObject.TryGetComponent(out ITroopAssembly character))
                        character.SetAssemblyPoint(tapPos);
                }
            }
        }
    }

    public interface IGlobalInteract
    {
        public void OnDoubleTap(Vector3 tapPos);
    }
}