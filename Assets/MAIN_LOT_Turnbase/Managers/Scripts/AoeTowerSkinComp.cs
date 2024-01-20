// using FOW;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTowerSkinComp : MonoBehaviour, ISkinComp
    {
        [SerializeField] private Transform m_SkinAnchor;

        private IAnimateComp _animateComp;
        
        public void Init(string skinAddress)
        {
            if (skinAddress.Equals(""))
                return;

            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor, this);
        }

        public void Init(string skinAddress, IAnimateComp animateComp)
        {
            _animateComp = animateComp;
            if (skinAddress.Equals(""))
                return;

            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor, this);
        }

        public void SetDefaultMaterial()
        {
            Debug.Log("TODO: Tower refresh material for frozing");
        }

        public void SetCustomMaterial(Material material)
        {
            Debug.Log("TODO: Tower need material for frozing");
            
        }

        public void ReturnSkin(GameObject skinObject)
        {
            _animateComp.Init(skinObject);
        }

        public void TurnSkin(bool state)
        {
            m_SkinAnchor.gameObject.SetActive(state);
        }
    }
}