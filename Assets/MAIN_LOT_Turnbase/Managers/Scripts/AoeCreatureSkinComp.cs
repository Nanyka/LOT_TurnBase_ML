using UnityEngine;

namespace JumpeeIsland
{
    public class AoeCreatureSkinComp : MonoBehaviour, ISkinComp
    {
        [SerializeField] private Transform m_SkinAnchor;
        
        private IAnimateComp _curAnimComp;
        private Material _defaultMaterial;
        private Renderer m_Renderer;

        public void Init(string skinAddress)
        {
            Debug.Log("This is a blank function");
        }

        public void Init(string skinAddress, IAnimateComp animateComp)
        {
            if (skinAddress.Equals(""))
                return;
            
            // Reset spawnTransform
            for (int i = 0; i < m_SkinAnchor.childCount; i++)
                Destroy(m_SkinAnchor.GetChild(i).gameObject);

            _curAnimComp = animateComp;
            AddressableManager.Instance.GetAddressableGameObject(skinAddress, m_SkinAnchor, this);
        }

        public void SetDefaultMaterial()
        {
            m_Renderer.material = _defaultMaterial;
        }

        public void SetCustomMaterial(Material material)
        {
            m_Renderer.material = material;
        }

        public void ReturnSkin(GameObject skinObject)
        {
            _curAnimComp.Init(skinObject);
            m_Renderer = skinObject.GetComponentInChildren<Renderer>();
            _defaultMaterial = m_Renderer.material;
        }
    }
}