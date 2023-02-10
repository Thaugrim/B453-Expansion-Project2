using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using NaughtyAttributes;
using UnityEngine;

public enum FaceDirection
{
    Left,
    Right,
}

public enum EntityKind { Mouse, Rabbit, Boar }

[RequireComponent(typeof(EntityDataHolder))]
public partial class Entity : MonoBehaviour
{
    protected enum EntityState
    {
        Idle,
        ChasingPlayer,
        RunningFromPlayer,
        Attacking,
        CapturedByPlayer,
    }

    protected int _hp;

    public EntityKind Type;
    [SerializeField] protected Animator _anim;
    [SerializeField] protected BreatheMoveAnim moveStaggerAnim;
    
    [Header("Raycasting")]
    [SerializeField] private LayerMask _playerRaycastMask;
    [SerializeField] protected SpriteDirection _spriteDir;

    [Header("Test/Debug")]
    [SerializeField] private bool _showGizmos;
    [SerializeField] private bool _debugLogState;
    [SerializeField] private Color _testRayColor;

    [SerializeField] private EntityNavigation.NavigationMode _startNavMode;
    protected EntityNavigation _navigation;
    public EntityData Data { get; private set; }

    private PlayerController _cachedPlayer;
    private bool _playerInSight, _isAlive;
    public bool IsAlive => _isAlive;

    [ShowNonSerializedField] private EntityState _state;
    private Action _updateAction;
    private Transform CachedPlayerTransform => _cachedPlayer ? _cachedPlayer.transform : null;

    private FOVRaycastHelper<PlayerController> _fovRaycaster;
    
    private EntityState PlayerSeenState =>
        Data.Damage > 0
            ? EntityState.ChasingPlayer
            : EntityState.RunningFromPlayer;

    public static event Action<Entity> OnEntityDeath;
    
    private void Awake()
    {
        InitData();
        Data.OnValidated += OnValidate;
    }
    private void Start()
    {
        InitState();
        _hp = Data.Hp;
        GameManager.Instance.AllEntities.Add(this);
        OnEntityDeath += GameManager.Instance.OnEntityDie;
    }

    private void OnValidate()
    {
        if (_spriteDir == null) _spriteDir = GetComponentInChildren<SpriteDirection>();
        if (_anim == null) _anim = GetComponentInChildren<Animator>();
        if (moveStaggerAnim == null) moveStaggerAnim = GetComponentInChildren<BreatheMoveAnim>();
        
        InitData();
    }
    private void OnDisable()
    {
        OnEntityDeath -= GameManager.Instance.OnEntityDie;
        _navigation.OnReachedDestination -= OnNavigationReachedDestination;
    }

    protected void InitData()
    {
        Data = GetComponent<EntityDataHolder>().Data;
        _navigation = GetComponent<EntityNavigation>();
        _updateAction = UpdateIdleState;
        _fovRaycaster = new FOVRaycastHelper<PlayerController>(transform, _playerRaycastMask);
        _navigation.OnReachedDestination += OnNavigationReachedDestination;

        if (Application.isPlaying)
        {
            _navigation.Speed = Data.EntitySpeed;
        }
    }
    private IEnumerator CachPlayer()
    {
        while (!GameManager.Instance.PlayerController)
        {
            yield return null;
            InitState();
        }
    }
    protected void InitState()
    {
        moveStaggerAnim.enabled = false;
        
        if (_startNavMode == EntityNavigation.NavigationMode.MoveToPlayer)
        {
            _cachedPlayer = GameManager.Instance.PlayerController;
        }

        _navigation.SetState(CachedPlayerTransform, _startNavMode);
        
        TransitionToIdle(EntityState.Idle);
    }

    private void Update()
    {
        UpdatePlayerInSight();
        _updateAction();
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos) return;
     
        if (!Data) InitData(); // hack for edit mode
        Gizmos.color = _testRayColor;
        foreach (var rayDir in GetRayDirections())
        {
            Gizmos.DrawRay(transform.position, rayDir * Data.ViewDistance);
        }
    }

    /// <param name="damage">true if entity is still alive</param>
    public bool TakeDamage(int damage)
    {
        _hp -= damage;

        if (_entityDebug)
            Debug.Log($"Entity *({name}) TakeDamage({damage}). Hp is now {_hp}", gameObject);

        if (_hp <= 0)
        {
            Die();
            return false;
        }

        return true;
    }

    public void CaptureEntity() => State = EntityState.CapturedByPlayer;
    public void ReleaseEntity() => State = EntityState.Idle;

    protected void Die()
    {
        OnEntityDeath?.Invoke(this);
    }

    private void UpdatePlayerInSight()
    {
        _cachedPlayer = RayCastForPlayer();

        PlayerController player = _cachedPlayer;
        bool foundPlayer = player != null;

        if (player) _cachedPlayer = player;

        if (foundPlayer && !_playerInSight)
        {
            OnPlayerFound();
        }
        else if (!foundPlayer && _playerInSight)
        {
            OnPlayerLost();
        }

        _playerInSight = foundPlayer;
    }
    
    private PlayerController RayCastForPlayer()
    {
        IEnumerable<Vector3> rayDirections = GetRayDirections();
        return _fovRaycaster.RayCastForPlayer(rayDirections, Data.ViewDistance);
    }

    private void OnPlayerFound()
    {
        if (_entityDebug)
            Debug.Log(LogStr("Found player!"), gameObject);
    }

    private void OnPlayerLost()
    {
        if (_entityDebug)
            Debug.Log(LogStr("Where player?!"), gameObject);
    }

    private IEnumerable<Vector3> GetRayDirections() =>
        _fovRaycaster.GetRayDirections(_spriteDir.Vector, Data.ViewFOVAngle, Data.NumRays);



    private string LogStr(string msg) => $"{nameof(Entity)}: {msg}";

    private bool IsPlayerInAttackRange()
    {
        var distToPlayer = Vector2.Distance(CachedPlayerTransform.position, transform.position);
        var isInAttackRange = distToPlayer < Data.AttackRange;
        return isInAttackRange;
    }
}