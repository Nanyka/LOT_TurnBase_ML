using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;

public class SingleJumperController : MonoBehaviour
{
<<<<<<< HEAD
    public bool UseThisTurn;

    [SerializeField] private EnvironmentController _environmentController;
    [SerializeField] private AgentManager m_AgentManager;
=======
    [SerializeField] protected EnvironmentController _environmentController;
    [SerializeField] protected AgentManager m_AgentManager;
>>>>>>> testSkillManager
    [SerializeField] private Collider _platformColider;
    [SerializeField] protected Transform _rotatePart;
    [SerializeField] private MeshRenderer _agentRenderer;
    [SerializeField] private List<Material> _agentColor;
    [SerializeField] private float _showLocalReward;

    protected Agent m_Agent;
    private int _platformMaxCol;
    private int _platformMaxRow;
    private Vector3 _platformPos;
<<<<<<< HEAD
    private Transform _mTransfrom;
    private Transform _smallGoalTransform;
    private Transform _largeGoalTransform;
=======
    protected Transform _mTransform;
>>>>>>> testSkillManager
    private (Vector3 targetPos, int jumpStep) _mMoving;
    private int _steps;
    private int _currentDirection;
    private Vector3 _defaultPos;
<<<<<<< HEAD
    private bool _isMoved;
=======
    private bool _isUseThisTurn;
>>>>>>> testSkillManager

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

    public virtual void OnEnable()
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

    // REFACTORING: Access this information from EnvironmentManager
    private void SetUpPlatform()
    {
        var platformSize = _platformColider.bounds.size;
        _platformMaxCol = Mathf.RoundToInt((platformSize.x - 1) / 2);
        _platformMaxRow = Mathf.RoundToInt((platformSize.z - 1) / 2);
    }

    #region Connect with BRAIN

    // Move the agent periodically
    public virtual void AskForAction()
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
<<<<<<< HEAD
    public void ResponseAction(ActionBuffers responseAction)
    {
        _currentDirection = responseAction.DiscreteActions[0];
        m_AgentManager.CollectUnitResponse(responseAction.DiscreteActions[1]); // finish this action and turn to the next agent
        // MoveDirection();
=======
    public virtual void ResponseAction(int direction)
    {
        _currentDirection = direction;
        MoveDirection();
        m_AgentManager.CollectUnitResponse(); // finish this action and turn to the next agent
>>>>>>> testSkillManager
    }

    /// <summary>
    /// Controls the movement of the GameObject based on the actions received from agent manager.
    /// </summary>
    /// <param name="direction"></param>
    public virtual void MoveDirection()
    {
        _mMoving = GetPositionByDirection(_currentDirection);

        // Change agent direction before the agent jump to the new position
        if (_mMoving.targetPos != _mTransfrom.position)
            _rotatePart.transform.forward = _mMoving.targetPos - _mTransfrom.position;
        else
            m_Agent.AddReward(-0.01f); // punish agent when it stand in place beside the edges

        _mTransfrom.position = _mMoving.targetPos;
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
        if (!CheckInBoundary(newPos)) return (curPos, jumpCount);
        
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

    protected Vector3 DirectionToVector(int direction)
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

    protected bool CheckAvailableMove(Vector3 checkPos)
    {
        return _environmentController.FreeToMove(checkPos) && CheckInBoundary(checkPos);
    }

    protected bool CheckInBoundary(Vector3 checkPos)
    {
        return Mathf.Abs(checkPos.x - _platformPos.x) <= _platformMaxCol &&
               Mathf.Abs(checkPos.z - _platformPos.z) <= _platformMaxRow;
    }

    #endregion

    public virtual void ResetAgent()
    {
<<<<<<< HEAD
        // m_Agent.EndEpisode(); // just use this one when training one one agent
        _mTransfrom.position = _defaultPos;
=======
        _mTransform.position = _defaultPos;
>>>>>>> testSkillManager
        _mMoving.targetPos = _defaultPos;
        _agentRenderer.material = _agentColor[0];
        _isUseThisTurn = false;
    }

    #region GET & SET
    
    public MovementCalculator GetMovementCalculator()
    {
        return _environmentController.GetMovementCalculator();
    }

    public Material GetDefaultMaterial()
    {
        return _agentRenderer.material;
    }

    public Agent GetAgent()
    {
        return m_Agent;
    }

    public Vector3 GetPosition()
    {
        return _mTransfrom.position;
    }

    public virtual Vector3 GetDirection()
    {
        return _rotatePart.transform.forward;
    }

    public virtual int GetJumpStep()
    {
        return _mMoving.jumpStep;
    }

    public void ChangeColor(int index)
    {
        _agentRenderer.material = _agentColor[Mathf.Clamp(index, 0, _agentColor.Count - 1)];
    }

<<<<<<< HEAD
    private void UpdateLocalReward()
    {
        _showLocalReward = m_Agent.GetCumulativeReward();
    }
=======
    public void SetMaterial(Material setMaterial)
    {
        _agentRenderer.material = setMaterial;
    }

    public bool CheckUsedThisTurn()
    {
        return _isUseThisTurn;
    }

    protected void MarkAsUsedThisTurn()
    {
        _isUseThisTurn = true;
    }

    public void ResetMoveState()
    {
        _isUseThisTurn = false;
    }

    public void ResetMoveState(Material factionMaterial)
    {
        _isUseThisTurn = false;
        _agentRenderer.material = factionMaterial;
    }

    #endregion
>>>>>>> testSkillManager
}