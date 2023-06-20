using UnityEngine;

namespace GOAP
{
    [CreateAssetMenu(fileName = "ResourcData", menuName = "Resource Data", order = 51)]
    public class ResourceData : ScriptableObject
    {
        public string ResourceTag;
        public string ResourceQueue;
        public string ResourceState;
    }
}
