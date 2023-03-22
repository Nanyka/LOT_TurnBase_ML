using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class SingleJumperController : MonoBehaviour
{
    public bool UseThisTurn;
    
    [SerializeField] private EnvironmentController _environmentController;
    [SerializeField] private AgentManager m_AgentManager;
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
    private int _currentDirection;
    private Vector3 _defaultPos;

    public void Awake()
    {
        // Since this example does not inherit from the Agent class, explicit registration
        // of the RpcCommunicator is required. The RPCCommunicator should only be compiled
        // for Standalone platforms (i.e. Windows, Linux, or Mac)
#if UNITY_EDITOR || UNITY_STANDALONE
        if (!CommunicatorFactory.CommunicatorRegistered)
        {
            CommunicatorFactory.Register<ICommunicator>(RpcCommunicator.Create);
        }
#endif
    }

    public void OnEnable()
    {
        m_Agent = GetComponent<Agent>();
        _mTransfrom = transform;
        _defaultPos = _mTransfrom.position;
        _platformPos = _platformColider.transform.position;
        _platformPos = new Vector3(_platformPos.x, 0f, _platformPos.z);
        _mMoving.targetPos = _defaultPos;
        // _mMoving.targetPos = _defaultPos + _platformPos;

        SetUpPlatform();
    }
    
    // REFACTORING: Access this information from AgentManager
    private void SetUpPlatform()
    {
        var platformSize = _platformColider.bounds.size;
        _platformMaxCol = Mathf.RoundToInt((platformSize.x - 1) / 2);
        _platformMaxRow = Mathf.RoundToInt((platformSize.z - 1) / 2);
    }

    #region Connect with BRAIN

    // Move the agent periodically
    public void AskForAction()
    {
        if (m_Agent == null)
            return;

        if (Academy.Instance.IsCommunicatorOn)
            m_Agent?.RequestDecision();
        else
            StartCoroutine(WaitToRequestDecision());
    }

    private IEnumerator WaitToRequestDecision()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        m_Agent?.RequestDecision();
    }

    // Receive action decision from ActuatorComponent
    public void ResponseAction(int direction)
    {
        _currentDirection = direction;
        MoveDirection();
    }

    /// <summary>
    /// Controls the movement of the GameObject based on the actions received from agent manager.
    /// </summary>
    /// <param name="direction"></param>
    public void MoveDirection()
    {
        _mMoving = GetPositionByDirection(_currentDirection);
        
        // Change agent direction before the agent jump to the new position
        _agentRenderer.transform.forward = _mMoving.targetPos - _mTransfrom.position;
        _mTransfrom.position = _mMoving.targetPos;

        // TODO: remove this set reward for jumping, add reward when agent attack enemy successfully
        // if (_mMoving.jumpStep > 0)
        // {
        //     ChangeColor();
        //     m_AgentManager.ContributeGroupReward(_mMoving);
        // }
        
        m_AgentManager.CollectUnitResponse(); // finish this action and turn to the next agent
    }

    #endregion

    #region Moving function
    
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
            if (_environmentController.CheckObjectInTeam(newPos, m_AgentManager.GetFaction()))
                jumpCount++;
            else
                jumpCount++;

            curPos = newPos + DirectionToVector(direction);
            newPos = curPos + DirectionToVector(direction);

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
                checkVector += Vector3.left;
                break;
            case 1:
                checkVector += Vector3.right;
                break;
            case 2:
                checkVector += Vector3.back;
                break;
            case 3:
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

    public void ResetAgent()
    {
        // m_Agent.EndEpisode(); // just use this one when training one one agent
        _mTransfrom.position = _defaultPos;
        _mMoving.targetPos = _defaultPos;
        // _mMoving.targetPos = _defaultPos + _platformPos;
        _agentRenderer.material = _agentColor[0];
    }

    public Agent GetAgent()
    {
        return m_Agent;
    }

    public Vector3 GetPosition()
    {
        return _mTransfrom.position;
    }

    public Vector3 GetDirection()
    {
        return _agentRenderer.transform.forward;
    }

    public int GetJumpStep()
    {
        return _mMoving.jumpStep;
    }

    public void ChangeColor(int index)
    {
        _agentRenderer.material = _agentColor[Mathf.Clamp(index, 0, _agentColor.Count - 1)];
    }
}