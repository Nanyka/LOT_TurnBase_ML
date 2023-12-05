using System;
using System.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace JumpeeIsland
{
    public class AddressableManager : Singleton<AddressableManager>
    {
        private string m_LogPrefab;

        public Sprite GetAddressableSprite(string objectKey)
        {
            m_LogPrefab = objectKey;

#if UNITY_STANDALONE_OSX
            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;
#endif

            if (m_LogPrefab.IsNullOrWhitespace())
                return null;

            var handle = Addressables.LoadAssetAsync<Sprite>(m_LogPrefab);
            return handle.WaitForCompletion();
        }

        public ScriptableObject GetAddressableSO(string objectKey)
        {
            m_LogPrefab = objectKey;

#if UNITY_STANDALONE_OSX
            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;
#endif

            var handle = Addressables.LoadAssetAsync<ScriptableObject>(m_LogPrefab);
            return handle.WaitForCompletion();
        }

        public void GetAddressableGameObject(string objectKey, Transform spawnTransform)
        {
            m_LogPrefab = objectKey;

#if UNITY_STANDALONE_OSX
            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;
#endif

            // Reset spawnTransform
            foreach (Transform child in spawnTransform)
                Destroy(child.gameObject);

            Addressables.InstantiateAsync(m_LogPrefab, spawnTransform);
        }

        // Get skin for animated objects
        public void GetAddressableGameObject(string objectKey, Transform spawnTransform, SkinComp skinComp,
            AnimateComp animateComp)
        {
            m_LogPrefab = objectKey;

#if UNITY_STANDALONE_OSX
            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;
#endif

            // Reset spawnTransform
            foreach (Transform child in spawnTransform)
                Destroy(child.gameObject);

            var handle = Addressables.InstantiateAsync(m_LogPrefab, spawnTransform);
            var skin = handle.WaitForCompletion();
            handle.Completed += delegate { if (handle.Status == AsyncOperationStatus.Succeeded) animateComp.Init(skin); };
            // animateComp.SetAnimator(skin.GetComponent<Animator>());

            var factionPart = skin.transform.Find("FactionPart");
            if (factionPart != null)
                for (int i = 0; i < factionPart.childCount; i++)
                    if (factionPart.GetChild(i).TryGetComponent(out Renderer part))
                        skinComp.SetFactionRenderer(part);
        }

        private void AddPrivateToken(UnityWebRequest request)
        {
            string bucketAccessToken = "313a05c186c7fc63e17fc91fc00265a7"; // Add your bucket token here

#if UNITY_STANDALONE_OSX
  bucketAccessToken = "313a05c186c7fc63e17fc91fc00265a7";
#endif

            var encodedToken = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($":{bucketAccessToken}"));

            request.SetRequestHeader("Authorization", $"Basic {encodedToken}");
        }

        public Material GetMaterial(string objectKey)
        {
            m_LogPrefab = objectKey;

#if UNITY_STANDALONE_OSX
            //Add private token to addressable web request header
            Addressables.WebRequestOverride = AddPrivateToken;
#endif

            var handle = Addressables.LoadAssetAsync<Material>(m_LogPrefab);
            return handle.WaitForCompletion();
        }
    }
}