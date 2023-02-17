using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName = "ScriptableObject/Data/Player Data", order = 20)]
public class PlayerData : EntityData
{
    [Header("Player Values")]
    [SerializeField] private bool _isWeak = true;
    public bool IsWeak => _isWeak;

    private const int _strongValue = 9;
    public int StrongValue => _strongValue;

    [SerializeField] private float _biteDistance = 3f, _biteOffset = 1f, _biteTime = 0.5f;
    public float BiteDistance => _biteDistance;
    public float BiteOffset => _biteOffset;
    public float BiteTime => _biteTime;

    [SerializeField] private float _biteSpeedWhileWeak = 0.75f, _biteSpeedWhileStrong = 1f;
    public float MoveToTargetDurationWhileWeak => _moveToTargetDurationWhileWeak;
    public float MoveToTargetDurationWhileStrong => _moveToTargetDurationWhileStrong;

    [SerializeField] private float _moveToTargetDurationWhileWeak = 1.25f, _moveToTargetDurationWhileStrong = 1f;  // 1.25f, 1f
    public float MoveBackFromTargetDurationWhileWeak => _moveBackFromTargetDurationWhileWeak;
    public float MoveBackFromTargetDurationWhileStrong => _moveBackFromTargetDurationWhileStrong;

    [SerializeField] private float _moveBackFromTargetDurationWhileWeak = 0.75f, _moveBackFromTargetDurationWhileStrong = 0.5f;
    public float BiteSpeedWhileWeak => _biteSpeedWhileWeak;
    public float BiteSpeedWhileStrong => _biteSpeedWhileStrong;

    [Header("Player Sprites")]
    [SerializeField] private Sprite _weakSprite;
    [SerializeField] private Sprite _strongSprite;
    public Sprite WeakSprite => _weakSprite;
    public Sprite StrongSprite => _strongSprite;

    [SerializeField] private Sprite _weakAttackingSprite, _strongAttackingSprite;
    public Sprite WeakAttackingSprite => _weakAttackingSprite;
    public Sprite StrongAttackingSprite => _strongAttackingSprite;

    [SerializeField] private Sprite[] _weakEatingAnimation, _strongEatingAnimation;
    public Sprite[] WeakEatingAnimation => _weakEatingAnimation;
    public Sprite[] StrongEatingAnimation => _strongEatingAnimation;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve _moveToTargetCurveFailedBite;
    [SerializeField] private AnimationCurve _moveBackFromTargetCurveFailedBite;
    public AnimationCurve MoveToTargetCurveFailedBite => _moveToTargetCurveFailedBite;
    public AnimationCurve MoveBackFromTargetCurveFailedBite => _moveBackFromTargetCurveFailedBite;

    [SerializeField] private AnimationCurve _moveToTargetCurveBiteSuccess, _moveBackFromTargetCurveBiteSuccess;
    public AnimationCurve MoveToTargetCurveBiteSuccess => _moveToTargetCurveBiteSuccess;
    public AnimationCurve MoveBackFromTargetCurveBiteSuccess => _moveBackFromTargetCurveBiteSuccess;
}
