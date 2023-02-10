using System;
using DG.Tweening;
using UnityEngine;

public class BreatheMoveAnim : MonoBehaviour
{
    public enum Mode
    {
        LocalScale,
        LocalRotation,
    }

    [SerializeField] private Mode _mode;
    [SerializeField] private Transform _trans;
    [SerializeField] private Vector3 _dirVector;
    [SerializeField] private float _deltaValue;
    [SerializeField] private float _moveOutTime;
    // [SerializeField] private AnimationCurve _moveOutEase;
    [SerializeField] private AnimationCurve _ease;

    [SerializeField] private float _moveBackTime;
    // [SerializeField] private AnimationCurve _moveBackEase;
        
    private Tween _runningTween;
    private Vector3 _origEuler;
    private Vector3 _origScale;

    private void OnValidate()
    {
        if (_trans == null) _trans = transform;
        if (enabled)
        {
            StartAnim();   
        }
    }

    private void OnEnable()
    {
        _origEuler = transform.eulerAngles;
        _origScale = transform.localScale;
        
        StartAnim();
    }

    private void StartAnim()
    {
        _runningTween?.Kill();
        if (_dirVector == Vector3.zero) return;

        var orig = _mode switch
        {
            Mode.LocalScale => _origScale,
            Mode.LocalRotation => _origEuler,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var (left, right) = GetTargets(orig);
        
        _runningTween = DOTween.Sequence()
            .Append(TweenFunc(right, _moveOutTime))
            .Append(TweenFunc(orig, _moveBackTime))
            .Append(TweenFunc(left, _moveOutTime))
            .Append(TweenFunc(orig, _moveBackTime))
            .SetEase(_ease)
            .SetLoops(-1);
    }

    private (Vector3 leftVec, Vector3 rightVec) GetTargets(Vector3 orig)
    {
        var halfAngle = _deltaValue * 0.5f;
        var targetRight = orig + _dirVector * halfAngle;
        var targetLeft = orig - _dirVector * halfAngle;
        return (targetLeft, targetRight);
    }

    private Tween TweenFunc(Vector3 target, float duration, AnimationCurve ease = null)
    {
        Tween tween;
        switch (_mode)
        {
            case Mode.LocalScale:
                tween = _trans.DOScale(target, duration);
                if (ease != null) tween = tween.SetEase(ease);
                return tween;
            case Mode.LocalRotation:
                tween = _trans.DOLocalRotate(target, duration);
                if (ease != null) tween = tween.SetEase(ease);
                return tween;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void OnDisable()
    {
        if (_runningTween != null) _runningTween.Kill();
    }

    private void Reset()
    {
        switch (_mode)
        {
            case Mode.LocalScale:
                _trans.localScale = _origEuler;
                break;
            case Mode.LocalRotation:
                _trans.eulerAngles = _origEuler;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
}