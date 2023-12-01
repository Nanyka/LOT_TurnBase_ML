using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FOW
{
    public abstract class FogOfWarRevealer : MonoBehaviour
    {
        [Header("Customization Variables")]
        [SerializeField] public float ViewRadius = 15;
        [SerializeField] public float AdditionalSoftenDistance = 0;

        [Range(0, 360)]
        [SerializeField] public float ViewAngle = 360;

        [SerializeField] public float UnobscuredRadius = 1f;

        [SerializeField] protected bool AddCorners;

        //[SerializeField] public bool RevealHidersInFadeOutZone = true;
        [Range(0,1)]
        [SerializeField] public float RevealHiderInFadeOutZonePercentage = .5f;

        [Tooltip("how high above this object should the sight be calculated from")]
        [SerializeField] public float EyeOffset = 0;
        [SerializeField] public float VisionHeight = 3;
        [SerializeField] public float VisionHeightSoftenDistance = 1.5f;

        [Header("Technical Variables")]
        [SerializeField] protected LayerMask ObstacleMask;
        [SerializeField] public float RaycastResolution = .5f;

        [Range(1, 30)]
        [Tooltip("Higher values will lead to more accurate edge detection, especially at higher distances. however, this will also result in more raycasts.")]
        [SerializeField] protected int MaxEdgeResolveIterations = 10;

        [Range(0, 10)]
        [SerializeField] protected int NumExtraIterations = 4;

        [Range(0, 5)]
        [SerializeField] protected int NumExtraRaysOnIteration = 5;

        [Tooltip("Static revealers are revealers that need the sight function to be called manually, similar to the 'Called Elsewhere' option on FOW world. To change this at runtime, use the SetRevealerAsStatic(bool IsStatic) Method.")]
        [SerializeField] public bool StartRevealerAsStatic = false;
        [HideInInspector] public bool StaticRevealer = false;

        [HideInInspector]
        public int FogOfWarID;
        [HideInInspector]
        public int IndexID;

        //local variables
        //protected List<ViewCastInfo> ViewPoints = new List<ViewCastInfo>();
        protected FogOfWarWorld.CircleStruct CircleStruct;
        protected bool IsRegistered = false;
        protected ViewCastInfo[] ViewPoints;
        [HideInInspector] public int NumberOfPoints;
        [HideInInspector] public float[] Angles;
        [HideInInspector] public float[] Radii;
        [HideInInspector] public bool[] AreHits;

        [Header("debug, you shouldnt have to mess with this")]
        [Range(.001f, 1)]
        [Tooltip("Lower values will lead to more accurate edge detection, especially at higher distances. however, this will also result in more raycasts.")]
        [SerializeField] protected float MaxAcceptableEdgeAngleDifference = .005f;
        [SerializeField] protected float EdgeDstThreshold = 0.1f;
        [SerializeField] protected float DoubleHitMaxDelta = 0.1f;
        [SerializeField] protected float DoubleHitMaxAngleDelta = 15;
#if UNITY_EDITOR
        [SerializeField] public bool DebugMode = false;
        [SerializeField] protected bool LogNumRaycasts = false;
        [SerializeField] protected int NumRayCasts;
        [SerializeField] protected float DrawRayNoise = 0;
        [SerializeField] protected bool DrawExtraCastLines;
        [SerializeField] protected bool DrawIteritiveLines;
#endif
        public List<FogOfWarHider> hidersSeen = new List<FogOfWarHider>();

        public struct ViewCastInfo
        {
            public bool hit;
            public Vector3 point;
            public float dst;
            public float angle;
            public Vector3 normal;
            public Vector3 direction;

            public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle, Vector3 _normal, Vector3 dir)
            {
                hit = _hit;
                point = _point;
                dst = _dst;
                angle = _angle;
                normal = _normal;
                direction = dir;
            }
        }

        public struct EdgeInfo
        {
            public ViewCastInfo minViewCast;
            public ViewCastInfo maxViewCast;
            public bool shouldUse;

            public EdgeInfo(ViewCastInfo _pointA, ViewCastInfo _pointB, bool _shouldUse)
            {
                minViewCast = _pointA;
                maxViewCast = _pointB;
                shouldUse = _shouldUse;
            }
        }

        private void OnEnable()
        {
            //ViewPoints = new ViewCastInfo[FogOfWarWorld.instance.maxPossibleSegmentsPerRevealer];

            //Angles = new float[ViewPoints.Length];
            //Radii = new float[ViewPoints.Length];
            //AreHits = new bool[ViewPoints.Length];
            RegisterRevealer();
        }

        private void OnDisable()
        {
            DeregisterRevealer();
        }

        public void RegisterRevealer()
        {
            if (StartRevealerAsStatic)
                SetRevealerAsStatic(true);
            else
                SetRevealerAsStatic(false);     //fail-safe in case someone changes the value in debug mode

            NumberOfPoints = 0;
            if (FogOfWarWorld.instance == null)
            {
                if (!FogOfWarWorld.RevealersToRegister.Contains(this))
                {
                    FogOfWarWorld.RevealersToRegister.Add(this);
                }
                return;
            }

            if (IsRegistered)
            {
                Debug.Log("Tried to double register revealer");
                return;
            }

            IsRegistered = true;
            FogOfWarID = FogOfWarWorld.instance.RegisterRevealer(this);

            ViewPoints = new ViewCastInfo[FogOfWarWorld.instance.maxPossibleSegmentsPerRevealer];

            Angles = new float[ViewPoints.Length];
            Radii = new float[ViewPoints.Length];
            AreHits = new bool[ViewPoints.Length];
            CircleStruct = new FogOfWarWorld.CircleStruct();
            CalculateLineOfSight();
            //_RegisterRevealer();
        }

        public void DeregisterRevealer()
        {
            if (FogOfWarWorld.instance == null)
            {
                if (FogOfWarWorld.RevealersToRegister.Contains(this))
                {
                    FogOfWarWorld.RevealersToRegister.Remove(this);
                }
                return;
            }
            if (!IsRegistered)
            {
                //Debug.Log("Tried to de-register revealer thats not registered");
                return;
            }
            foreach (FogOfWarHider hider in hidersSeen)
            {
                hider.RemoveSeer(this);
            }
            hidersSeen.Clear();
            IsRegistered = false;
            FogOfWarWorld.instance.DeRegisterRevealer(this);
        }

        public void SetRevealerAsStatic(bool IsStatic)
        {
            if (IsRegistered)
            {
                if (StaticRevealer && !IsStatic)
                    FogOfWarWorld.instance.numDynamicRevealers++;
                else if (!StaticRevealer && IsStatic)
                    FogOfWarWorld.instance.numDynamicRevealers--;
            }
            
            StaticRevealer = IsStatic;
        }

        public void CalculateLineOfSight()
        {
            NumberOfPoints = 0;
            _CalculateLineOfSight();
        }
        protected abstract void _CalculateLineOfSight();

        public void RevealHiders()
        {
            _RevealHiders();
        }
        protected abstract void _RevealHiders();

        protected void AddViewPoint(ViewCastInfo point)
        {
            if (NumberOfPoints < ViewPoints.Length)
                ViewPoints[NumberOfPoints] = point;
            NumberOfPoints++;
        }

        public bool TestPoint(Vector3 point)
        {
            return _TestPoint(point);
        }
        protected abstract bool _TestPoint(Vector3 point);
    }

}
