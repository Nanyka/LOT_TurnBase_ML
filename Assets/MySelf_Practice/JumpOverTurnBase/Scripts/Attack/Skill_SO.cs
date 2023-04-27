using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Skill", menuName = "TurnBase/Skill", order = 1)]
public class Skill_SO : ScriptableObject
{
    //Manage attribute of unit's skills
    [Header("Skill variable")] 
    [SerializeField] private string _animTrigger;
    [SerializeField] private float _magnitude;
    
    [Header("Skill range")]
    [SerializeField] private RangeType _rangeType;
    [SerializeField] private int _numberOfSteps;

    [Header("ML property")] [SerializeField]
    private NNModel MLModel;

    private SkillRange _skillRange;

    #region Skill Range

    public Vector3 InvokeAt(Vector3 currPos, Vector3 direction)
    {
        CheckSkillRangeNull();

        return _skillRange.InvokeAt(currPos, direction);
    }

    public IEnumerable<Vector3> CalculateSkillRange(Vector3 currPos, Vector3 direction)
    {
        direction = direction.normalized;
        CheckSkillRangeNull();
        
        return _skillRange.CalculateSkillRange(currPos, direction, _numberOfSteps);
    }

    private void CheckSkillRangeNull()
    {
        if (_skillRange == null)
            SetRangeType();
    }

    #endregion

    #region Initiate
    
    private void SetRangeType()
    {
        switch (_rangeType)
        {
            case RangeType.StraightAheadSingle:
                _skillRange = new StraightAheadSingle();
                break;
            case RangeType.FrontArea:
                _skillRange = new FrontArea();
                break;
            case RangeType.BackHandStrike:
                _skillRange = new BackHandStrike();
                break;
            case RangeType.FrontArea3D:
                _skillRange = new FrontArea3D();
                break;
        }
    }

    #endregion

    public float GetMagnitude()
    {
        return _magnitude;
    }

    public NNModel GetModel()
    {
        return MLModel;
    }

    public string GetAnimation()
    {
        return _animTrigger;
    }
}