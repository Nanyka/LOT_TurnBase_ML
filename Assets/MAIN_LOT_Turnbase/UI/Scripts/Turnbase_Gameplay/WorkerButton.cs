using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class WorkerButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IConfirmFunction
    {
        [SerializeField] private TextMeshProUGUI m_Level;
        [SerializeField] private Image m_ItemIcon;

        // private AsyncOperationHandle m_UCDObjectLoadingHandle;
        private AoeWorkerMenu _mAoeWorkerMenu;
        private Vector3 _spawnPosition;
        private JIInventoryItem _inventoryItem;
        private CurrencyUnit _cost = new();
        private int _layerMask = 1 << 6;
        private Camera _camera;

        private void OnEnable()
        {
            _camera = Camera.main;
        }

        public void TurnOn(JIInventoryItem creatureInventory, AoeWorkerMenu aoeTroopMenu)
        {
            _inventoryItem = creatureInventory;
            m_ItemIcon.sprite = AddressableManager.Instance.GetAddressableSprite(_inventoryItem.spriteAddress);
            
            var costs = SavingSystemManager.Instance.GetPurchaseCost(_inventoryItem.virtualPurchaseId);
            if (costs.Count > 0)
            {
                _cost.currencyId = costs[0].id;
                _cost.amount = costs[0].amount;
            }
                
            m_Level.text = _cost.amount.ToString();
            
            if (_mAoeWorkerMenu == null)
                _mAoeWorkerMenu = aoeTroopMenu;
        }

        public void TurnOff()
        {
            gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _mAoeWorkerMenu.StartADeal(_inventoryItem.skinAddress[0]);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(eventData.position);
            if (!Physics.Raycast(ray, out var collidedTile, 100f, _layerMask))
                return;
            
            _spawnPosition = collidedTile.point;
            _spawnPosition = new Vector3(Mathf.RoundToInt(_spawnPosition.x),
                Mathf.RoundToInt(_spawnPosition.y), Mathf.RoundToInt(_spawnPosition.z));
            
            _mAoeWorkerMenu.SelectLocation(_spawnPosition);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _mAoeWorkerMenu.EndDeal(this);
        }

        public void ClickYes()
        {
            var newWorker = new CreatureData
            {
                EntityName = _inventoryItem.inventoryName,
                Position = _spawnPosition,
                CurrentLevel = 0,
                CreatureType = CreatureType.WORKER
            };
            SavingSystemManager.Instance.OnTrainACreature(newWorker, _cost);
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}