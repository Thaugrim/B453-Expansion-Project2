using UnityEngine;

[CreateAssetMenu(fileName = "New CommonEntityData", menuName = "ScriptableObject/Data/Common Entity Data", order = 21)]
public class CommonEntityData : ScriptableObject
{
    [Tooltip("Multiplier to set Rigidbody velocity")]
    [field: SerializeField] public int SpeedModifier { get; private set; } = 100; // Player only

    [field: SerializeField] public float EntityAttackTimeDelta { get; set; } = 2f;
    [field: SerializeField] public float EntitySpeedModifier { get; private set; } = 1f;

    [Header("Stat calculation")]
    [SerializeField] private int _killPointsDivider;
    [SerializeField] private int _absorptionPointsDivider;
    
    [field: Header("Entity view raycasting")]
    [field: SerializeField] public float BaseFOVAngle { get; private set; }
    [field: SerializeField] public float BaseViewDistance { get; private set; }
    [field: SerializeField] public float MaxDistanceBetweenRays { get; private set; } = 0.25f;

    [field: Header("Entity view \"lvlups\"")]
    [field: SerializeField] public float FOVAnglePerVisionPoint { get; private set; }
    [field: SerializeField] public float ViewDistancePerVisionPoint { get; private set; }
    
    public int KillPointsDivider => _killPointsDivider;
    public int AbsorptionPointsDivider => _absorptionPointsDivider;
}