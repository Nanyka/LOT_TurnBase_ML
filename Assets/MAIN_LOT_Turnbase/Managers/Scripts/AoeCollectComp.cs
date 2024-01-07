using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace JumpeeIsland
{
    // Remove this entity and spawn the intended object
    public class AoeCollectComp : MonoBehaviour, IRemoveEntity, ICollectComp
    {
        [SerializeField] private ParticleSystem _collectingVfx;
        [SerializeField] private bool _isGlobalVfx;
        [SerializeField] private GlobalVfxType _globalVfxType;
        [SerializeField] private bool _isForBattleMode;

        private AoeCollectableEntity _collectableEntity;
        private bool _isCollected;

        public void Init(AoeCollectableEntity collectableEntity)
        {
            _collectableEntity = collectableEntity;
            MainUI.Instance.OnEnableInteract.AddListener(EnableTrigger);
        }

        private void EnableTrigger()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isCollected)
                return;

            _isCollected = true;
            
            if (_isGlobalVfx)
                GameFlowManager.Instance.AskGlobalVfx(_globalVfxType,Vector3.zero);
            
            _collectableEntity.ContributeCommands();
            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);
            StartCoroutine(DestroyVisual());
        }
        
        private IEnumerator DestroyVisual()
        {
            // Add VFX
            yield return new WaitForSeconds(2f);
            gameObject.SetActive(false);
        }

        public GameObject GetRemovedObject()
        {
            return gameObject;
        }

        public EntityData GetEntityData()
        {
            return _collectableEntity.GetData();
        }
    }

    public interface ICollectComp
    {
        public void Init(AoeCollectableEntity collectableEntity);
    }
}