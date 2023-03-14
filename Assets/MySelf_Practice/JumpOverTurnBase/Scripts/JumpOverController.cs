using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;
using Random = UnityEngine.Random;

public class JumpOverController : MonoBehaviour
{
    public float timeBetweenDecisionsAtInference;
    float m_TimeSinceDecision;

    [SerializeField] private EnvironmentController _environmentController;
    [SerializeField] private int _faction;
    [SerializeField] private int _maxStep;
    [SerializeField] private Collider _platformColider;
    [SerializeField] private MeshRenderer _agentRenderer;
    [SerializeField] private List<Material> _agentColor;

    private Agent m_Agent;
    private int _platformMaxCol;
    private int _platformMaxRow;
    private Vector3 _platformPos;
    private Transform _mTransfrom;
    private Transform _smallGoalTransform;
    private Transform _largeGoalTransform;
    private (Vector3 targetPos, int jumpStep) _mMoving;
    private int _steps;

    public void Awake()
    {
        // Since this example does not inherit from the Agent class, explicit registration
        // of the RpcCommunicator is required. The RPCCommunicator should only be compiled
        // for Standalone platforms (i.e. Windows, Linux, or Mac)
#if UNITY_EDITOR || UNITY_STANDALONE
        if (!CommunicatorFactory.CommunicatorRegistered)
        {
            Debug.Log("Registered Communicator.");
            CommunicatorFactory.Register<ICommunicator>(RpcCommunicator.Create);
        }
#endif
    }

    public void OnEnable()
    {
        m_Agent = GetComponent<Agent>();
        _environmentController.OnChangeFaction.AddListener(AskForAction);
        _mTransfrom = transform;
        _platformPos = _platformColider.transform.position;
        _platformPos = new Vector3(_platformPos.x, 0f, _platformPos.z);

        SetUpPlatform();

        ResetEnvironment();
    }

    // Move the agent periodically
    private void AskForAction()
    {
        if (m_Agent == null)
            return;

        if (Academy.Instance.IsCommunicatorOn)
            m_Agent?.RequestDecision();
        else if (_environmentController.GetCurrFaction() == _faction)
            StartCoroutine(WaitToRequestDecision());
    }

    private IEnumerator WaitToRequestDecision()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        m_Agent?.RequestDecision();
    }

    private void SetUpPlatform()
    {
        var platformSize = _platformColider.bounds.size;
        _platformMaxCol = Mathf.RoundToInt((platformSize.x - 1) / 2);
        _platformMaxRow = Mathf.RoundToInt((platformSize.z - 1) / 2);
    }

    #region Moving function

    /// <summary>
    /// Controls the movement of the GameObject based on the actions received.
    /// </summary>
    /// <param name="direction"></param>
    public void MoveDirection(int direction)
    {
        _mMoving = GetPositionByDirection(direction);
        _mTransfrom.position = _mMoving.targetPos;

        m_Agent.AddReward(-0.5f);

        if (_mMoving.jumpStep > 0)
        {
            _agentRenderer.material = _agentColor[Mathf.Clamp(_mMoving.jumpStep,0,_agentColor.Count-1)];
            m_Agent.AddReward(1f* (_mMoving.jumpStep + Mathf.Pow(1 + 1f,_mMoving.jumpStep)));

            if (_environmentController.IsRunOutOfObstacle())
            {
                m_Agent.EndEpisode();
                ResetAgent();
            }
        }

        _environmentController.ChangeFaction();
        _environmentController.OnChangeFaction.Invoke();

        _steps++;
        if (_steps == _maxStep)
        {
            m_Agent.EndEpisode();
            ResetAgent();
        }
    }

    private (Vector3, int) GetPositionByDirection(int direction)
    {
        var curPos = _mMoving.targetPos;
        var newPos = curPos + DirectionToVector(direction);

        return MovingPath(curPos, newPos, direction, 0);
    }

    private (Vector3, int) MovingPath(Vector3 curPos, Vector3 newPos, int direction, int jumpCount)
    {
        if (CheckAvailableMove(newPos))
        {
            if (jumpCount == 0)
                return (newPos, jumpCount);
            return (curPos, jumpCount);
        }

        if (CheckAvailableMove(newPos + DirectionToVector(direction)))
        {
            _environmentController.DestroyObstacle(newPos);
            curPos = newPos + DirectionToVector(direction);
            newPos = curPos + DirectionToVector(direction);
            jumpCount++;
            return MovingPath(curPos, newPos, direction, jumpCount);
        }

        return (curPos, jumpCount);
    }

    private Vector3 DirectionToVector(int direction)
    {
        var checkVector = Vector3.zero;

        switch (direction)
        {
            case 0:
                break;
            case 1:
                checkVector += Vector3.left;
                break;
            case 2:
                checkVector += Vector3.right;
                break;
            case 3:
                checkVector += Vector3.back;
                break;
            case 4:
                checkVector += Vector3.forward;
                break;
        }

        return checkVector;
    }

    private bool CheckAvailableMove(Vector3 checkPos)
    {
        return Mathf.Abs(checkPos.x - _platformPos.x) <= _platformMaxCol &&
               Mathf.Abs(checkPos.z - _platformPos.z) <= _platformMaxRow &&
               _environmentController.FreeToMove(checkPos);
    }

    #endregion

    private void ResetAgent()
    {
        ResetEnvironment();
    }

    private void ResetEnvironment()
    {
        _steps = 0;

        _mMoving.targetPos = Vector3.zero + _platformPos;
        _mTransfrom.position = _mMoving.targetPos;
        _agentRenderer.material = _agentColor[0];
        // _environmentController.AskToSpawnObstacle();
        _environmentController.KickOffEnvironment();
    }
}