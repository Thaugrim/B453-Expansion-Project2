using System;
using System.Linq;
using Extensions;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;
    public static MapManager Instance
    {
        get => _instance ??= FindObjectOfType<MapManager>();
        private set => _instance = value;
    }
    
    [SerializeField] private Transform _randomPlacesParent;
    [SerializeField] private Transform _villagerSpawnParent;
    [SerializeField] private bool _mapDebug = false;

    private Transform[] _randomPlaces;
    private Transform[] _villagerSpawns;

    private Transform[] RandomPlaces => _randomPlaces ??= _randomPlacesParent.Cast<Transform>().ToArray();
    private Transform[] VillagerSpawnPoints => _villagerSpawns ??= _villagerSpawnParent.Cast<Transform>().ToArray();
    
    public Transform GetRandomPlaceTransform() => RandomPlaces.GetRandom();
    public Vector3 GetRandomVillagerSpawnPosition() => VillagerSpawnPoints.GetRandom().position;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Vector3 GetRandomRunawayPlace(Vector3 entityPos, Vector3 playerTransformPosition, Vector3 lastDestination)
    {
        bool VectorIsNotLast(Vector3 vec)
        {
            return Vector3.Distance(vec, lastDestination) > 0.1f;
        }
        
        var filteredPlaces =  _randomPlaces.Select(trans => trans.position).Where(pos =>
        {
            var entityToTargetVec = pos - entityPos;
            var playerChaseVec = entityPos - playerTransformPosition;

            return Vector2.Dot(entityToTargetVec, playerChaseVec) > 0f;
        }).Where(VectorIsNotLast).ToArray();

        var chosen = filteredPlaces.Length > 0
            ? filteredPlaces.GetRandom()
            : RandomPlaces
                .Select(trans => trans.position)
                .Where(VectorIsNotLast)
                .ToArray()
                .GetRandom();

        // var entityToTargetVec = chosen - entityPos;
        // var playerChaseVec = entityPos - playerTransformPosition;
        // Debug.Log($"Chosen point: EntityPos: {entityPos}, PlayerPos: {playerTransformPosition}, entityToTargetVec: {entityToTargetVec}, playerChaseVec: {playerChaseVec}. Dot: {Vector2.Dot(entityToTargetVec, playerChaseVec)}");

        if (_mapDebug)
            Debug.Log($"$Chosen Runaway pos: {chosen} Last: {lastDestination}");
        return chosen;
    }
}
