using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

public class FogOfWarController : MonoBehaviour
{
    [SerializeField] private float _viewSize;
    [SerializeField] private float _viewScaleMult;

    private RectTransform RectTrans => (RectTransform) transform;

    public float BaseViewSize
    {
        get => _viewSize;
        set
        {
            _viewSize = value;
            UpdateScale();
        }
    }

    private void OnValidate() => UpdateScale();

    
    // TODO: set scale + config 
    // (Or... use a shaderGraph instead...
    // (underworld graph has PolarCoord->mul->power that does something pretty similar. Maybe lose the power node)
    private void UpdateScale()
    {
        // This sets transform scale rather than rect.. relevant?
        // var scale = _viewSize = _viewSize * _viewScaleMult;
        // transform.SetLocalScaleX(scale);
    }
}
