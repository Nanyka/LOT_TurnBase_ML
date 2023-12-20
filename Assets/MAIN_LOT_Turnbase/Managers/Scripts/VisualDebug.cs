using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JumpeeIsland
{
    public class VisualDebug : Singleton<VisualDebug>
    {
        [SerializeField] private LineRenderer _debugLine;
        [SerializeField] private Transform _debugPoint;

        public void DrawLine(IEnumerable<Vector3> corners)
        {
            _debugLine.positionCount = corners.Count();

            for (int i = 0; i < _debugLine.positionCount; i++)
                _debugLine.SetPosition(i, corners.ElementAt(i));
        }

        public void ShowPointAt(Vector3 atPos)
        {
            _debugPoint.position = atPos;
        }
    }
}