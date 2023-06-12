using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BasicXZController : MonoBehaviour
{
    public float timeBetweenDecisionsAtInference;
    float m_TimeSinceDecision;

    public GameObject largeGoal;
    public GameObject smallGoal;
    public int _currIndex;
    public int _smallIndex;
    public int _largeIndex;
    [SerializeField] private int _maxStep;
    [SerializeField] private Collider _platformColider;
    private Vector3 _currPos = Vector3.zero;
    private Dictionary<int, Vector3> _platformDic = new();

    Agent m_Agent;
    private int _platformMaxCol;
    private int _platformMaxRow;
    private Vector3 _platformPos;
    private Transform _mTransfrom;
    private Transform _smallGoalTransform;
    private Transform _largeGoalTransform;

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
        _platformPos = _platformColider.transform.position;
        _platformPos = new Vector3(_platformPos.x, 0f, _platformPos.z);

        _mTransfrom = transform;
        _smallGoalTransform = smallGoal.transform;
        _largeGoalTransform = largeGoal.transform;

        SetUpPlatform();

        ResetEnvironment();
    }

    private void SetUpPlatform()
    {
        var platformSize = _platformColider.bounds.size;
        _platformMaxCol = Mathf.RoundToInt((platformSize.x - 1) / 2);
        _platformMaxRow = Mathf.RoundToInt((platformSize.z - 1) / 2);

        _platformDic.Clear();
        int index = 0;
        foreach (var zAxis in Enumerable.Range(-_platformMaxRow, Mathf.RoundToInt(platformSize.z)))
        {
            foreach (var xAxis in Enumerable.Range(-_platformMaxCol, Mathf.RoundToInt(platformSize.x)))
            {
                _platformDic.Add(index++, new Vector3(xAxis, 0f, zAxis) + _platformPos);
            }
        }
    }

    /// <summary>
    /// Controls the movement of the GameObject based on the actions received.
    /// </summary>
    /// <param name="direction"></param>
    public void MoveDirection(int direction)
    {
        _currPos = GetPositionByIndex(direction);
        _mTransfrom.position = _currPos;

        m_Agent.AddReward(-0.1f);

        if (_smallGoalTransform.position == _currPos)
        {
            m_Agent.AddReward(1f);
            m_Agent.EndEpisode();
            ResetAgent();
        }

        if (_largeGoalTransform.position == _currPos)
        {
            m_Agent.AddReward(5f);
            m_Agent.EndEpisode();
            ResetAgent();
        }

        _steps++;
        if (_steps == _maxStep)
        {
            m_Agent.EndEpisode();
            ResetAgent();
        }
    }

    private Vector3 GetPositionByIndex(int index)
    {
        var checkVector = Vector3.zero;

        switch (index)
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

        checkVector += _currPos;
        
        if (Mathf.Abs(checkVector.x - _platformPos.x) > _platformMaxCol ||
            Mathf.Abs(checkVector.z - _platformPos.z) > _platformMaxRow)
            return _currPos;
        
        switch (index)
        {
            case 0:
                break;
            case 1:
                _currIndex--;
                break;
            case 2:
                _currIndex++;
                break;
            case 3:
                _currIndex -= (_platformMaxCol * 2 + 1);
                break;
            case 4:
                _currIndex += (_platformMaxCol * 2 + 1);
                break;
        }

        return _platformDic[_currIndex];
    }

    public void ResetAgent()
    {
        // // This is a very inefficient way to reset the scene. Used here for testing.
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // m_Agent = null; // LoadScene only takes effect at the next Update.
        // // We set the Agent to null to avoid using the Agent before the reload

        ResetEnvironment();
    }

    private void ResetEnvironment()
    {
        _steps = 0;

        _currPos = Vector3.zero + _platformPos;
        _currIndex = _platformDic.FirstOrDefault(x => x.Value == _currPos).Key;
        _mTransfrom.position = _currPos;

        _smallIndex = Random.Range(0, GetLengthOfList());
        _largeIndex = Random.Range(0, GetLengthOfList());
        _smallGoalTransform.position = _platformDic[_smallIndex];
        _largeGoalTransform.position = _platformDic[_largeIndex];
    }

    public void FixedUpdate()
    {
        WaitTimeInference();
    }

    void WaitTimeInference()
    {
        if (m_Agent == null)
        {
            return;
        }

        if (Academy.Instance.IsCommunicatorOn)
        {
            m_Agent?.RequestDecision();
        }
        else
        {
            if (m_TimeSinceDecision >= timeBetweenDecisionsAtInference)
            {
                m_TimeSinceDecision = 0f;
                m_Agent?.RequestDecision();
            }
            else
            {
                m_TimeSinceDecision += Time.fixedDeltaTime;
            }
        }
    }

    public int GetLengthOfList()
    {
        return _platformDic.Count;
    }
}