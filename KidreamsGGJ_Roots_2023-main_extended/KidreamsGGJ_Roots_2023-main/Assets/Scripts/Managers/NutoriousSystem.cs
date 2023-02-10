using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NutoriousSystem : MonoBehaviour
{
    [SerializeField]
    private  int nutoriousPoints = 0;
    [SerializeField]
    private int[] spawnMilestones;
    [SerializeField]
    private Entity entityToSpawn;
    [SerializeField]
    private MapManager mapManager;
    private void OnEnable()
    {
        Entity.OnEntityDeath += OnEntityDeath;
    }

    private void OnDisable()
    {
        Entity.OnEntityDeath -= OnEntityDeath;
    }

    private void OnEntityDeath(Entity obj)
    {
        var shouldSpawn = spawnMilestones.Any(milestone => milestone == nutoriousPoints);
        if(shouldSpawn)
        {
            var tempObj = Instantiate(entityToSpawn, mapManager.GetRandomVillagerSpawnPosition(), Quaternion.identity);
        }
        nutoriousPoints++;
    }
}