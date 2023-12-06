using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JumpeeIsland
{
    public class TroopDropButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IConfirmFunction
    {
        [SerializeField] private TextMeshProUGUI m_Level;
        [SerializeField] private Image m_ItemIcon;

        // private AsyncOperationHandle m_UCDObjectLoadingHandle;
        private DropTroopMenu m_DropTroopMenu;
        private CreatureData m_CreatureItem;
        private Vector3 _spawnPosition;
        private JIInventoryItem _inventoryItem;
        private int _layerMask = 1 << 6;
        private Camera _camera;

        private void OnEnable()
        {
            _camera = Camera.main;
        }

        public virtual void TurnOn(CreatureData creatureItem, DropTroopMenu dropTroopMenu)
        {
            m_CreatureItem = creatureItem;
            _inventoryItem = creatureItem.GetInventoryItem();
            m_ItemIcon.sprite = AddressableManager.Instance.GetAddressableSprite(_inventoryItem.spriteAddress);
            m_Level.text = creatureItem.CurrentLevel.ToString();
            // TODO add Stats address and set Health Slider here
            
            if (m_DropTroopMenu == null)
                m_DropTroopMenu = dropTroopMenu;
        }

        public void TurnOff()
        {
            gameObject.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            m_DropTroopMenu.StartADeal(_inventoryItem.skinAddress[0]);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(eventData.position);
            if (!Physics.Raycast(ray, out var collidedTile, 100f, _layerMask))
                return;
            
            m_DropTroopMenu.SelectLocation(collidedTile.transform.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var collidedTile, 100f, _layerMask))
                return;

            _spawnPosition = collidedTile.transform.position;
            m_DropTroopMenu.EndDeal(this);
        }

        public void ClickYes()
        {
            SavingSystemManager.Instance.OnTrainACreature(m_CreatureItem,_spawnPosition);
            
            // Remove item from CreatureMenu
            TurnOff();
            m_DropTroopMenu.CheckEmptyMenu();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool CheckActive()
        {
            return gameObject.activeInHierarchy;
        }
    }
}