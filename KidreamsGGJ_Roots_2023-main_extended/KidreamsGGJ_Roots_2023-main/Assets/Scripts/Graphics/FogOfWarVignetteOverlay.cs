using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class FogOfWarVignetteOverlay : MonoBehaviour
{
    [SerializeField] private Material _screenShaderMaterial;
    [SerializeField, Range(0f, 1f)] private float _viewValue;
    
    [Header("Tweening")]
    [SerializeField] private float _animTime;
    private AnimationCurve _animEaseCurve;
    
    [Header("Driven by view value")]
    [SerializeField, MinMaxSlider(0f, 1f)] private Vector2 _viewRadiusMinMax;
    [SerializeField, MinMaxSlider(0f, 50f)] private Vector2 _viewCrispnessMinMax;

    private List<Tween> _runningTweens = new();

    private static int PropScreenAspect = Shader.PropertyToID("_Screen_Aspect");
    // private static int PropOffset = Shader.PropertyToID("_FogOfWar_Offset");
    private static int PropBlend = Shader.PropertyToID("_FogOfWar_Blend");
    private static int PropViewRadius = Shader.PropertyToID("_FogOfWar_ViewRadius");
    private static int PropCrispness = Shader.PropertyToID("_FogOfWar_Crispness");

    private void OnValidate()
    {
        SetViewValue(_viewValue);
    }

    private void OnEnable()
    {
        SetScreenAspect();
        _screenShaderMaterial.SetFloat(PropBlend, 1f);
    }

    private void OnDisable() => _screenShaderMaterial.SetFloat(PropBlend, 0f);


#if UNITY_EDITOR

    void Update()
    {
        SetScreenAspect(); // Dont break when resizing editor
    }
#endif


    /// <summary>
    /// Set how much the player will be able to see
    /// </summary>
    /// <param name="viewRadius">The radius the player can see (0 = nothing, 1 = full screen height is visible)</param>
    public void SetViewValue(float viewRadius)
    {
        _viewValue = viewRadius;
        
        _runningTweens.ForEach(tween => tween.Kill());
        _runningTweens.Clear();

        var targetRadius = Lerp(_viewRadiusMinMax, viewRadius);
        var targetCrispness = Lerp(_viewCrispnessMinMax, viewRadius);
        
        var radiusTween = _screenShaderMaterial.DOFloat(targetRadius, PropViewRadius, _animTime).SetEase(_animEaseCurve);
        var crispnessTween = _screenShaderMaterial.DOFloat(targetCrispness, PropCrispness, _animTime).SetEase(_animEaseCurve);
        
        _runningTweens.Add(radiusTween);
        _runningTweens.Add(crispnessTween);
    }

    private Tween SetEase(Tween tween, Ease ease, AnimationCurve curve)
    {
        if (ease == Ease.INTERNAL_Custom) return tween.SetEase(curve);
        return tween.SetEase(ease);
    }

    private static float Lerp(Vector2 minMax, float t) => Mathf.Lerp(minMax.x, minMax.y, t);
    
    
    private void SetScreenAspect() =>
        _screenShaderMaterial.SetFloat(PropScreenAspect, (float)Screen.width / Screen.height);
}
