using System;
using NaughtyAttributes;
using UnityEngine;

public enum EntityType
{
    Animal,
    Human,
}
    
[CreateAssetMenu(fileName = "New Entity", menuName = "ScriptableObject/Data/Entity Data", order = 21)]
public class EntityData : ScriptableObject
{
    public enum Stat
    {
        Hp,
        Damage,
        Speed,
        Vision,
    }
    [ValidateInput(nameof(ValidateNotNull))]
    [SerializeField, Expandable] private CommonEntityData commonData;

    [SerializeField] private int _maxHp;
    [SerializeField] private int _damage;
    [SerializeField] private int _speed;
    [SerializeField] private int _vision;
    [SerializeField] private float _attackRange;

    [SerializeField] private Stat[] absorbedStats;
    
    [field: SerializeField] public string Name { get; private set; }

    public int Hp => _maxHp;
    public int Damage => _damage;
    public int PlayerSpeed => _speed;
    public int EntitySpeed => Mathf.FloorToInt(_speed * commonData.EntitySpeedModifier);
    public int Vision => _vision;
    public float AttackRange => _attackRange;
    public Stat[] AbsorbedStats => absorbedStats;
    public CommonEntityData CommonData => commonData;

    public int CalculatedSpeed => PlayerSpeed * commonData.SpeedModifier;
    public event Action OnValidated;

    private void OnValidate()
    {
        OnValidated?.Invoke();
    }

    // Entity View Raycasting - formulas
    [ShowNativeProperty] public float ViewDistance
    {
        get
        {
            if (Vision == 0) return 0;
            return commonData.BaseViewDistance + Vision * commonData.ViewDistancePerVisionPoint;
        }
    }

    [ShowNativeProperty] public float ViewFOVAngle
    {
        get
        {
            if (Vision == 0) return 0;
            return commonData.BaseFOVAngle + Vision * commonData.FOVAnglePerVisionPoint;
        }
    }
    
    public int GetStat(Stat stat)
    {
        return stat switch
        {
            Stat.Hp => Hp,
            Stat.Damage => Damage,
            Stat.Speed => PlayerSpeed,
            Stat.Vision => Vision,
            _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
        };
    }


    // Entity View raycasting - dependent (calculated from FOV/Distance)
    [ShowNativeProperty] public int NumRays => Mathf.CeilToInt(ViewFOVAngle / DeltaAngleRays);
    public float DeltaAngleRays => Mathf.Atan2(commonData.MaxDistanceBetweenRays, ViewDistance) * Mathf.Rad2Deg;


    private bool ValidateNotNull(CommonEntityData x) => x != null;

}
