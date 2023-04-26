using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StatHelper
{
    // calc:
    // Villagers: all stats point
    // BaseStat + (
    public static int GetHp(PlayerData myData, int damageTaken, IEnumerable<EntityData> killedEntities, EntityData absorbedEntity, CommonEntityData commonData)
        => GetStatAbsorbed(myData.Hp, killedEntities, absorbedEntity, commonData, EntityData.Stat.Hp) - damageTaken;

    public static int GetMana(PlayerData myData, int manaSpent, IEnumerable<EntityData> killedEntities, EntityData absorbedEntity, CommonEntityData commonData)
        => GetStatAbsorbed(myData.Mana, killedEntities, absorbedEntity, commonData, EntityData.Stat.Mana) - manaSpent;

    public static int GetSpeed(PlayerData myData, IEnumerable<EntityData> killedEntities, EntityData absorbedEntity, CommonEntityData commonData)
        => GetStatAbsorbed(myData.PlayerSpeed, killedEntities, absorbedEntity, commonData, EntityData.Stat.Speed);

    public static int GetDamage(PlayerData myData, IEnumerable<EntityData> killedEntities, EntityData absorbedEntity, CommonEntityData commonData)
        => GetStatAbsorbed(myData.Damage, killedEntities, absorbedEntity, commonData, EntityData.Stat.Damage);

    public static int GeSpeed(PlayerData myData, IEnumerable<EntityData> killedEntities, EntityData absorbedEntity, CommonEntityData commonData)
        => GetStatAbsorbed(myData.PlayerSpeed, killedEntities, absorbedEntity, commonData, EntityData.Stat.Speed);

    public static int GetVision(PlayerData myData, IEnumerable<EntityData> killedEntities, EntityData absorbedEntity, CommonEntityData commonData)
        => GetStatAbsorbed(myData.Vision, killedEntities, absorbedEntity, commonData, EntityData.Stat.Vision);

    private static int GetStatAbsorbed(int baseStat, 
        IEnumerable<EntityData> killedEntities, EntityData absorbedEntity, CommonEntityData commonData,
        EntityData.Stat stat)
    {

        var killedPoints = GetKillPointsForStat(killedEntities, stat);
        var absorbedPoints = GetStatFromEntity(stat, absorbedEntity);

        var killStat = killedPoints / commonData.KillPointsDivider;
        var absorbedStat = absorbedPoints / commonData.AbsorptionPointsDivider;

        var res = baseStat + killStat + absorbedStat;
        return Mathf.FloorToInt(res);
    }

    private static int GetKillPointsForStat(IEnumerable<EntityData> killedEntities, EntityData.Stat stat)
    {
        return killedEntities.Count(
            data => data != null && data.AbsorbedStats.Contains(stat)
        );
    }

    private static int GetStatFromEntity(EntityData.Stat stat, EntityData data)
    {
        if (!data) return 0;
        if (data.AbsorbedStats.Contains(stat)) return data.GetStat(stat);
        return 0;
    }
}