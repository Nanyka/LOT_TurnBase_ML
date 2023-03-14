using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCUnit : MonoBehaviour
{
    [SerializeField] private EnvironmentController _environmentController;
    [SerializeField] private Supervisor _supervisor;
    [SerializeField] private Collider _platformColider;
    [SerializeField] private MeshRenderer _agentRenderer;
    [SerializeField] private List<Material> _agentColor;
    [SerializeField] private float _currReward;

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
    
    public void OnEnable()
    {
        _mTransfrom = transform;
        _defaultPos = _mTransfrom.position;
        _platformPos = _platformColider.transform.position;
        _platformPos = new Vector3(_platformPos.x, 0f, _platformPos.z);
        _mMoving.targetPos = _defaultPos;
        // _mMoving.targetPos = _defaultPos + _platformPos;

        SetUpPlatform();
    }
    
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
        _supervisor.FinishAndResponse();
    }

    // Control unit movement from supervisor
    public void MoveDirection(int direct)
    {
        var previousPos = _mMoving.targetPos;
        _mMoving = GetPositionByDirection(direct);
        _mTransfrom.position = _mMoving.targetPos;

        // set punishment whenever agent stand in place
        if (Vector3.Distance(previousPos, _mMoving.targetPos) < Mathf.Epsilon)
        {
            _supervisor.AddIdlePunishment(); // If unit stand in place give it a punishment
        }

        else if (_mMoving.jumpStep > 0)
        {
            // _agentRenderer.material = _agentColor[Mathf.Clamp(_mMoving.jumpStep, 0, _agentColor.Count - 1)];
            _supervisor.AddReward(0.5f * Mathf.Pow(1 + 1f, _mMoving.jumpStep));
        }
        
        _supervisor.FinishAndResponse();
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
            if (_environmentController.CheckObjectInTeam(newPos, _supervisor.GetFaction()))
                jumpCount++;
            else
            {
                jumpCount++;
                overEnemy++;
            }

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

    #endregion

    public void ResetUnit()
    {
        _mTransfrom.position = _defaultPos;
        _mMoving.targetPos = _defaultPos;
        // _mMoving.targetPos = _defaultPos + _platformPos;
        _agentRenderer.material = _agentColor[0];
    }
}
