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
            
            _mAoeWorkerMenu.SelectLocation(collidedTile.transform.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var collidedTile, 100f, _layerMask))
                return;

            _spawnPosition = collidedTile.transform.position;
            _mAoeWorkerMenu.EndDeal(this);
        }

        public void ClickYes()
        {
            SavingSystemManager.Instance.OnTrainACreature(_inventoryItem ,_spawnPosition, false);
        }

        public Entity GetEntity()
        {
            throw new System.NotImplementedException();
        }

        public bool CheckActive()
        {
            return gameObject.activeInHierarchy;
        }
    }
}