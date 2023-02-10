using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(EntityDataHolder))]
public class EntityNavigation : MonoBehaviour
{
    public enum NavigationMode
    {
        MoveRandomly,
        MoveToPlayer,
        RunFromPlayer,
    }
    [Header("AI Agent Data and setting")]
    [SerializeField] private NavMeshAgent agent;
    private EntityData _data;

    [NaughtyAttributes.ShowNativeProperty]
    public NavigationMode NavMode { get; private set; }
    
    [SerializeField] private Transform _playerTransform;
    [Header("RandomRoaming")]
    private float distanceToNextTarget;

    [Header("Debug")]
    [SerializeField] private bool _aiDebug = false;
    [SerializeField] private bool _showGizmos;
    [SerializeField] private Color _gizmoColor;

    public event Action OnReachedDestination;
    
    public float Speed
    {
        get => agent.speed;
        set => agent.speed = value;
    }

    private Vector3 Destination
    {
        get => agent.destination;
        set
        {
            // Debug.Log($"Setting agent destination: {value}");
            agent.destination = value;
        }
    }

    
    public void SetState(Transform playerTransform, NavigationMode navState)
    {
        if (_aiDebug)
            Debug.Log($"EntityNavigation ({name}) SetState: {navState} (player: {playerTransform})", gameObject);

        if (playerTransform == null && navState == NavigationMode.MoveToPlayer)
        {
            Debug.LogError("Player is null");
        }
        NavMode = navState;
        _playerTransform = playerTransform;

        Destination = GetDestination(playerTransform);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        _data = GetComponent<EntityDataHolder>().Data;
    }

    private void Start()
    {
        InitAgent();
    }

    private void OnEnable()
    {
        agent.enabled = true;
    }

    private void OnDisable()
    {
        agent.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if(!_showGizmos || !Application.isPlaying) return;
        Gizmos.color = _gizmoColor;
        Gizmos.DrawSphere(Destination, 0.5f);
    }

    private void Update()
    {
        switch (NavMode)
        {
            case NavigationMode.MoveRandomly:
            case NavigationMode.RunFromPlayer:
                if (IsAgentCloseToTarget())
                {
                    var dest = GetDestination(_playerTransform);
                    SetAgentDestination(dest);
                    OnReachedDestination?.Invoke();
                }
                
                break;
            case NavigationMode.MoveToPlayer:
                SetAgentDestination(GetDestination(_playerTransform));
                // TODO: Need callback for reached player?
                // Need something here?
                break;
        }
    }

    private bool IsAgentCloseToTarget()
    {
        return agent.remainingDistance <= 0.1f;
    }

    private void InitAgent()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void SetAgentDestination(Vector3 destination)
    {
        agent.SetDestination(new Vector3(destination.x, destination.y, transform.position.z));
    }
    
    private Vector3 GetDestination(Transform playerTransform)
    {
        return NavMode switch
        {
            NavigationMode.MoveRandomly => MapManager.Instance.GetRandomPlaceTransform().position,
            NavigationMode.MoveToPlayer => playerTransform.position,
            NavigationMode.RunFromPlayer => MapManager.Instance.GetRandomRunawayPlace(transform.position, playerTransform.position, Destination),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}