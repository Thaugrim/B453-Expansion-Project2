using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FOVRaycastHelper<TTarget> where TTarget : Component
{
    private readonly Transform _raycastOriginTrans;
    
    private readonly RaycastHit2D[] _raycastResultsCache = new RaycastHit2D[100];
    private readonly LayerMask _layerMask;

    public TTarget CachedComponent { get; private set; }

    public FOVRaycastHelper(Transform raycastOriginTrans, LayerMask layerMask)
    {
        _raycastOriginTrans = raycastOriginTrans;
        _layerMask = layerMask;
    }
    
    public TTarget RayCastForPlayer(IEnumerable<Vector3> rayDirections, float distance)
    {
        foreach (Vector2 rayDir in rayDirections)
        {
            int size = Physics2D.RaycastNonAlloc(_raycastOriginTrans.position, rayDir, _raycastResultsCache, distance, _layerMask);

            if (size == 0)
                continue;
            
            TTarget player = SearchResults(_raycastResultsCache);

            if (player != null)
                return player;
        }

        return null;
    }
    
    private TTarget SearchResults(RaycastHit2D[] results)
    {
        if (results == null || results.Length == 0) return null;
        return results
            .Select(res =>
            {
                if (CachedComponent && CachedComponent.gameObject == res.transform.gameObject)
                    return CachedComponent;
                return res.transform.GetComponent<TTarget>();
            })
            .FirstOrDefault();
    }
    
    public IEnumerable<Vector3> GetRayDirections(Vector2 direction, float angleDegrees, int numRays)
    {
        Quaternion deltaRot = Quaternion.AngleAxis(angleDegrees / numRays, Vector3.forward);
        Vector3 curVec = Quaternion.AngleAxis(angleDegrees * 0.5f, Vector3.back) * direction;

        for (int i = 0; i < numRays; i++)
        {
            yield return curVec;
            curVec = deltaRot * curVec;
        }
    }
}