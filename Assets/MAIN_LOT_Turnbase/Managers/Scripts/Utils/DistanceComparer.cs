using System.Collections.Generic;
using UnityEngine;

namespace JumpeeIsland
{
    public class DistanceComparer : IComparer<Vector3>
    {
        private Vector3 target;

        public DistanceComparer(Vector3 target)
        {
            this.target = target;
        }

        public int Compare(Vector3 a, Vector3 b)
        {
            float distanceA = Vector3.Distance(a, target);
            float distanceB = Vector3.Distance(b, target);

            return distanceA.CompareTo(distanceB);
        }
    }
}