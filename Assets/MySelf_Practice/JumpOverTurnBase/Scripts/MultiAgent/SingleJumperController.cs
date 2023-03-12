using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class SingleJumperController : MonoBehaviour
{
    [SerializeField] private EnvironmentController _environmentController;
    [SerializeField] private AgentManager m_AgentManager;
    [SerializeField] private Collider _platformColider;
    [SerializeField] private MeshRenderer _agentRenderer;
    [SerializeField] private List<Material> _agentColor;

    Agent m_Agent;
    private int _platformMaxCol;
    private int _platformMaxRow;
    private Vector3 _platformPos;
    private Transform _mTransfrom;
    private Transform _smallGoalTransform;
    private Transform _largeGoalTransform;
    private (Vector3 targetPos, int jumpStep, int overEnemy) _mMoving;
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

    // REFACTORING: Access this information from AgentManager
    private void SetUpPlatform()
    {
        var platformSize = _platformColider.bounds.size;
        _platformMaxCol = Mathf.RoundToInt((platformSize.x - 1) / 2);
        _platformMaxRow = Mathf.RoundToInt((platformSize.z - 1) / 2);
    }

    #region Moving function

    // Receive action decision from ActuatorComponent
    public void ResponseAction(int direction)
    {
        _currentDirection = direction;
        m_AgentManager.ResponseBack();
    }

    /// <summary>
    /// Controls the movement of the GameObject based on the actions received from agent manager.
    /// </summary>
    /// <param name="direction"></param>
    public void MoveDirection()
    {
        _mMoving = GetPositionByDirection(_currentDirection);
        _mTransfrom.position = _mMoving.targetPos;

        if (_mMoving.jumpStep > 0)
        {
            // _agentRenderer.material = _agentColor[Mathf.Clamp(_mMoving.jumpStep, 0, _agentColor.Count - 1)];
            // m_Agent.AddReward(0.1f * (_mMoving.jumpStep + Mathf.Pow(2, _mMoving.jumpStep)));
            // _environmentController.OnPunishOppositeTeam.Invoke(m_AgentManager.GetFaction(), _mMoving.overEnemy);

            // v3.4: apply balance multiplier
            _agentRenderer.material = _agentColor[Mathf.Clamp(_mMoving.jumpStep, 0, _agentColor.Count - 1)];
            m_Agent.AddReward(0.1f * (_mMoving.jumpStep + Mathf.Pow(1 + 1f, _mMoving.jumpStep)) * BalanceMultiplier());
            _environmentController.OnPunishOppositeTeam.Invoke(m_AgentManager.GetFaction(), _mMoving.overEnemy);
        }
    }

    private (Vector3, int, int) GetPositionByDirection(int direction)
    {
        var curPos = _mMoving.targetPos;
        var newPos = curPos + DirectionToVector(direction);

        return MovingPath(curPos, newPos, direction, 0, 0);
    }

    private (Vector3, int, int) MovingPath(Vector3 curPos, Vector3 newPos, int direction, int jumpCount, int overEnemy)
    {
        if (CheckAvailableMove(newPos))
        {
            if (jumpCount == 0)
                return (newPos, jumpCount, overEnemy);
            return (curPos, jumpCount, overEnemy);
        }

        if (CheckAvailableMove(newPos + DirectionToVector(direction)))
        {
            // version 3.1
            if (_environmentController.CheckObjectInTeam(newPos, m_AgentManager.GetFaction()))
                jumpCount++;
            else
            {
                jumpCount++;
                overEnemy++;
            }
            
            // version 3.2
            // if (_environmentController.CheckObjectInTeam(newPos, m_AgentManager.GetFaction()))
            //     jumpCount++;
            // else
            // {
            //     jumpCount+=2;
            //     overEnemy++;
            // }

            curPos = newPos + DirectionToVector(direction);
            newPos = curPos + DirectionToVector(direction);

            return MovingPath(curPos, newPos, direction, jumpCount, overEnemy);
        }

        return (curPos, jumpCount, overEnemy);
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

    public void Punish(float amount)
    {
        m_Agent?.AddReward(-1f * amount);
    }

    #endregion

    public void ResetAgent()
    {
        m_Agent.EndEpisode();
        _mTransfrom.position = _defaultPos;
        _mMoving.targetPos = _defaultPos;
        // _mMoving.targetPos = _defaultPos + _platformPos;
        _agentRenderer.material = _agentColor[0];
    }

    public int GetCurrentAction()
    {
        return _currentDirection;
    }

    public Agent GetAgent()
    {
        return m_Agent;
    }

    // avoid abuse of using only single agent. When an agent collect too many reward, the adjacent movement will receive a less reward
    public float BalanceMultiplier()
    {
        if (m_AgentManager.GetMaxReward() - m_AgentManager.GetMinReward() == 0f)
            return 0f;
        
        return (m_AgentManager.GetMaxReward() - m_Agent.GetCumulativeReward()) /
               (m_AgentManager.GetMaxReward() - m_AgentManager.GetMinReward());
    }
}