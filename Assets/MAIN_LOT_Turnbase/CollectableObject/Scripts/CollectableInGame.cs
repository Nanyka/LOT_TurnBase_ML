using System.Collections;
using UnityEngine;

namespace JumpeeIsland
{
    public class CollectableInGame : MonoBehaviour, IRemoveEntity
    {
        [SerializeField] private CollectableEntity m_Entity;

        private CollectableController _collectableController;

        public void Init(CollectableData collectableData, CollectableController collectableController)
        {
            m_Entity.Init(collectableData);
            m_Entity.OnUnitDie.AddListener(DestroyEntity);
            transform.position = collectableData.Position;

            _collectableController = collectableController;
            _collectableController.AddCollectableToList(this);
        }
        
        public void DurationDeduct()
        {
            m_Entity.DurationDeduct();
        }
        
        private void DestroyEntity(Entity killedByEntity)
        {
            // just contribute commands when it is killed by player faction
            if (killedByEntity.GetFaction() == FactionType.Player)
                m_Entity.Collect();
            
            SavingSystemManager.Instance.OnRemoveEntityData.Invoke(this);
            StartCoroutine(DestroyVisual());
        }

        private IEnumerator DestroyVisual()
        {
            yield return new WaitForSeconds(1f);
            _collectableController.RemoveCollectable(this);
            gameObject.SetActive(false);
        }

        public void Remove(EnvironmentData environmentData)
        {
            environmentData.CollectableData.Remove((CollectableData)m_Entity.GetData());
        }
    }
}