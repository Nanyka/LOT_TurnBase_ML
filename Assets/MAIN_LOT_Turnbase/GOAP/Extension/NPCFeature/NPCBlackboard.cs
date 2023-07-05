using System.Collections;
using GOAP;
using UnityEngine;
using UnityEngine.Events;

public class NPCBlackboard : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Vector3> FollowTarget;
    [HideInInspector] public UnityEvent<float> SpeedUp;
    
    [SerializeField] private float _mRadius;
    [SerializeField] private GameObject _mTarget;
    [SerializeField] private Transform _makeBombArea;
    [SerializeField] private Transform _planBombArea;
    
    [Header("Thread analyser attributes")]
    [SerializeField] private float _speedupDuration = 3f;

    private Transform _mTransform;
    private readonly NPCThreatAnalyzer _npcThreatAnalyzer = new();

    private bool _isSpeedUp;

    private void Start()
    {
        _mTransform = transform;
    }

    #region MOVER

    public void SetTargetPos(Vector3 targetPos)
    {
        _mTarget.transform.position = targetPos;
    }

    #endregion

    #region THREAT ANALYSER
    
    // public void AnalyzeThreat(WorldStates targetStates)
    // {
    //     if (_copThreatAnalyzer.CheckThreatState(targetStates))
    //     {
    //         SpeedUp.Invoke(_speedupDuration);
    //         StartCoroutine(ResetSpeedUp());
    //     }
    // }
    //
    // private IEnumerator ResetSpeedUp()
    // {
    //     yield return new WaitForSeconds(_speedupDuration);
    //     _copThreatAnalyzer.RefreshedSpeedUp();
    // }

    #endregion
}
