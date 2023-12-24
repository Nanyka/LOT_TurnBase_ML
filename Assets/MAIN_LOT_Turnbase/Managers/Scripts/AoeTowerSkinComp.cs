// using FOW;
using UnityEngine;

namespace JumpeeIsland
{
    public class AoeTowerSkinComp : MonoBehaviour, ISkinComp
    {
        [SerializeField] private Transform m_SkinAnchor;
        
        public void Init(string skinAddress)
        {
            if (skinAddress.Equals(""))
                return;

            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor, this);
        }

        public void Init(string skinAddress, IAnimateComp animateComp)
        {
            throw new System.NotImplementedException();
        }

        public void SetDefaultMaterial()
        {
            throw new System.NotImplementedException();
        }

        public void SetCustomMaterial(Material material)
        {
            throw new System.NotImplementedException();
        }

        public void ReturnSkin(GameObject skinObject)
        {
            // var hider = GetComponent<IHiderDisable>();
            // if (skinObject.TryGetComponent(out FogOfWarHider fogHider))
            //     hider.SetHider(fogHider);
        }
    }
}