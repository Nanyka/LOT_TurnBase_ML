using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class AOEBuildingController : MonoBehaviour
    {
        protected List<IBuildingInfo> m_buildings = new();
    }

    public interface IBuildingInfo
    {
        
    }
}