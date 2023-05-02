using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using System;

public enum PlayerStates { Idle, Moving, Attacking, Biting, Eating, FailedBiting, Casting }

public class PlayerController : MonoBehaviour
{
    // state machine decleration
    protected delegate void PlayerState();
    protected PlayerState _playerState;

    public event Action OnDeath;

    [Header("Player Data")]
    [SerializeField, Expandable] private PlayerData _data;
    public float mana = 100;
    public PlayerData Data { get => _data; set => _data = value; }

    private bool _isWeak;
    public bool IsWeak => _isWeak;

    // TODO: Set kills/absorption absorbed entity when bite person / resurrect from grave
    [ShowNativeProperty] public int Hp => StatHelper.GetHp(_data, _damageTaken, _killedEntitiesData, AbsorbedEntity, _data.CommonData);
    [ShowNativeProperty] public int Mana => StatHelper.GetHp(_data, _manaSpent, _killedEntitiesData, AbsorbedEntity, _data.CommonData);  // Added for Spell Casting
    //[ShowNativeProperty] public int Damage => StatHelper.GetDamage(_data, _killedEntities, AbsorbedEntity, _data.CommonData);
    [ShowNativeProperty] public int Speed => StatHelper.GeSpeed(_data, _killedEntitiesData, AbsorbedEntity, _data.CommonData);
    [ShowNativeProperty] public int Vision => StatHelper.GetVision(_data, _killedEntitiesData, AbsorbedEntity, _data.CommonData);

    
    private int _currentStrongValue => Hp + Speed + Vision;

    public EntityData AbsorbedEntity;
    private readonly List<EntityData> _killedEntitiesData = new();
    private int _damageTaken; // separated from hp so we can calculate Absorbed enitty separately
    private int _manaSpent;

    // TODO: Test (stats work + with absorbed entity)

    [Header("Player Components")]
    [SerializeField] protected PlayerControls _playerControls;
    [SerializeField] protected SpriteRenderer _playerGraphics;
    [SerializeField] protected Rigidbody2D _rb;
    public Rigidbody2D Rb => _rb;

    //[SerializeField] private BreatheMoveAnim _moveAnim;
    [SerializeField] private bool _debugPlayerState;

    [Header("World Data")]
    [SerializeField] private LayerMask _biteLayer;

    [Header("Prefab Spellcasting")]
    [SerializeField] private GameObject _fireballPrefab;

    private Entity _lastPrey;
    private Vector2 _lastAttackingOriginPos, _lastTargetPos;
    private int _hpStatCounter = 0, _speedStatCounter = 0, _visionStatCounter = 0, _animationCounter = 0, _animationLenghtInFrames = 3; //_killCounterTest = 0;
    private float _moveToTargetDuration, _moveBackFromTargetDuration, _biteOffset;

    protected Vector2 _moveInput, _spriteDirection;
    protected InputAction _move, _bite;
    protected bool _isPlayingSound => CameraManager.Instance._cameraAudioSource.isPlaying;

    #region Monobehaviour Callbacks
    private void OnEnable()
    {
        _move = _playerControls.Player.Move;
        _move.Enable();

        _bite = _playerControls.Player.Bite;
        _bite.Enable();
        _bite.started += Bite;
    }
    private void Awake()
    {
        Initialize();
    }
    private void Start()
    {
        LaterInitialize();
    }
    private void Update()
    {
        _playerState.Invoke();
        if(Input.GetMouseButtonUp(0))
        {
            if (UseMana())
            {
                GameObject fireball = Instantiate(_fireballPrefab, transform.position, Quaternion.identity);
                Vector3 rot = Camera.main.WorldToViewportPoint(Input.mousePosition) - transform.position;
                rot.z = 0;

                fireball.transform.up = rot;
            }
        }

        if(mana < 100)
        {
            mana += 3f * Time.deltaTime;
            UIManager.Instance.UpdateMana(mana);
        }
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void OnDisable()
    {
        _move.Disable();
        _bite.Disable();
        OnDeath -= GameManager.Instance.OnPlayerDie;
    }
    #endregion

    protected virtual void Initialize()
    {
        _playerControls = new PlayerControls();
        _isWeak = _data.IsWeak;
        _moveToTargetDuration = _data.MoveToTargetDurationWhileWeak;
        _moveBackFromTargetDuration = _data.MoveBackFromTargetDurationWhileWeak;
        _biteOffset = _data.BiteOffset;
        
        _playerState = Idle;
    }
    protected virtual void LaterInitialize()
    {
        GameManager.Instance.PlayerController = this;
        UIManager.Instance.InitializePlayerUI(this);
        GameManager.Instance.UnderworldOverlay.SetRegularMode();
        OnDeath += GameManager.Instance.OnPlayerDie;
    }

    #region FixedUpdate Methods
    protected void Move()
    {
        Vector2 direction = new(_moveInput.x, _moveInput.y);
        _rb.velocity = _data.CalculatedSpeed * Time.fixedDeltaTime * direction;

        FlipSpriteToMoveDirection();
    }
    #endregion

    protected virtual void Bite(InputAction.CallbackContext biteContext)
    {
        _lastAttackingOriginPos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _spriteDirection, _data.BiteDistance, _biteLayer);

        if (hit)
        {
            _lastPrey = hit.transform.root.GetComponent<Entity>();

            if (!_lastPrey)
                return;

            Vector2 pos = (Vector2)transform.position + _spriteDirection * _data.BiteDistance;
            _lastTargetPos = new(pos.x, hit.transform.position.y);

            ChangeState(PlayerStates.Attacking);

            Debug.Log($"player {name} bite {hit.transform.root.name}");

        }
        else
        {
            Vector2 pos = (Vector2)transform.position + _spriteDirection * _data.BiteDistance;
            _lastTargetPos = new(pos.x, transform.position.y);

            ChangeState(PlayerStates.Attacking);

            Debug.Log($"player {name} didn't bite");
        }
    }

    protected virtual void Cast(InputAction.CallbackContext castContext)
    {
        Debug.Log("Casting from Callback");
    }

    #region States
    protected void Idle()
    {
        if (_debugPlayerState)
            //Debug.Log($"player state is Idle");

        //_killCounterTest = 0;
        _moveInput = _move.ReadValue<Vector2>();
        DoIdleAnimation();

        if (_lastPrey)
            _lastPrey = null;

        if (_moveInput != Vector2.zero)
            ChangeState(PlayerStates.Moving);
    }
    protected void Moving()
    {
        if (_debugPlayerState)
            //Debug.Log($"player state is Moving");

        _moveInput = _move.ReadValue<Vector2>();
        DoMovingAnimation();

        if (_moveInput == Vector2.zero)
            ChangeState(PlayerStates.Idle);
    }
    private void Attacking()
    {
        if (_debugPlayerState)
            //Debug.Log($"player state is Attacking");

        _moveInput = Vector2.zero;
        DoAttackAnimation();

        if (!_lastPrey)
        {
            transform.DOMoveX(_lastTargetPos.x, _moveToTargetDuration).SetEase(_data.MoveToTargetCurveBiteSuccess).OnComplete(() => ChangeState(PlayerStates.FailedBiting));
        }
        else
        {
            transform.DOMove(_lastTargetPos + new Vector2(_data.BiteOffset, 0f), _moveToTargetDuration).SetEase(_data.MoveToTargetCurveBiteSuccess).OnComplete(() => ChangeState(PlayerStates.Biting));
        }
    }
    private void Biting()
    {
        if (_debugPlayerState)
            //Debug.Log($"player state is Biting");

        _moveInput = Vector2.zero;
        DoBiteAnimation();

        if (!_lastPrey)
            ChangeState(PlayerStates.Idle);

        if (_lastPrey/* && _killCounterTest == 0*/)
        {
            if (!CameraManager.Instance.IsPlayingSounds)
            {
                CameraManager.Instance.ChangeAudioSource(CameraManager.Instance._bite);
                CameraManager.Instance._cameraAudioSource.Play();
            }
            
            _lastPrey.TakeDamage(_lastPrey.Data.Hp);
            //_killCounterTest++;
        }

        if (_lastPrey is not Villager)
        {
            transform.DOMove(_lastAttackingOriginPos, _moveBackFromTargetDuration).SetEase(_data.MoveBackFromTargetCurveBiteSuccess).OnComplete(() => ChangeState(PlayerStates.Eating));
        }
        else if (_lastPrey is Villager)
        {
            transform.DOMove(_lastAttackingOriginPos, _moveBackFromTargetDuration).SetEase(_data.MoveBackFromTargetCurveBiteSuccess).OnComplete(() => ChangeState(PlayerStates.Idle));
        }
        else
        {
            ChangeState(PlayerStates.Idle);
        }
    }
    private void Eating()
    {
        if (_debugPlayerState)
            //Debug.Log($"player state is Eating");

        _moveInput = Vector2.zero;
        DoEatingAnimation();

        if (!_lastPrey)
            ChangeState(PlayerStates.Idle);

        _animationCounter++;

        if (_animationCounter == _animationLenghtInFrames)
        {
            _animationCounter = 0;

            transform.DOMove(_lastAttackingOriginPos, _moveBackFromTargetDuration).SetEase(_data.MoveBackFromTargetCurveBiteSuccess).OnComplete(() => ChangeState(PlayerStates.Idle));
        }
        else
        {
            return;
        }
    }
    private void FailedBiting()
    {
        if (_debugPlayerState)
            //Debug.Log($"player state tried to Bite and failed");

        _moveInput = Vector2.zero;

        transform.DOMoveX(_lastAttackingOriginPos.x, _moveBackFromTargetDuration).SetEase(_data.MoveBackFromTargetCurveFailedBite).
            OnComplete(() => ChangeState(PlayerStates.Idle));
    }

    private void Casting()
    {
        Debug.Log("I'm casting!");
        ChangeState(PlayerStates.Idle);
    }
    #endregion

    private void TryChangeWeakness()
    {
        if (_currentStrongValue >= _data.StrongValue)
            _isWeak = false;
        else
            _isWeak = true;

        switch (IsWeak)
        {
            case true:
                _playerGraphics.sprite = _data.WeakSprite;
                _moveToTargetDuration = _data.MoveToTargetDurationWhileWeak;
                _moveBackFromTargetDuration = _data.MoveBackFromTargetDurationWhileWeak;
                break;
            case false:
                _playerGraphics.sprite = _data.StrongSprite;
                _moveToTargetDuration = _data.MoveToTargetDurationWhileStrong;
                _moveBackFromTargetDuration = _data.MoveBackFromTargetDurationWhileStrong;
                break;
        }
    }
    protected void FlipSpriteToMoveDirection()
    {
        if (_moveInput.x < 0)
        {
            _spriteDirection = new(-Mathf.Abs(transform.localScale.x), 0);
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (_moveInput.x > 0)
        {
            _spriteDirection = new(Mathf.Abs(transform.localScale.x), 0);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        _biteOffset = _data.BiteOffset * _spriteDirection.x;
    }
    protected void DoIdleAnimation()
    {
        TryChangeWeakness();

        if (_playerGraphics.sprite != _data.WeakSprite || _playerGraphics.sprite != _data.StrongSprite)
            _playerGraphics.sprite = _isWeak ? _data.WeakSprite : _data.StrongSprite;
    }
    protected void DoMovingAnimation()
    {
        //if (_playerGraphics.sprite == _data.WeakSprite || _playerGraphics.sprite == _data.StrongSprite) //|| movingWeak || movingStrong
            //_playerGraphics.sprite = _isWeak ? _data.WeakSprite : _data.StrongSprite;
    }
    private void DoAttackAnimation()
    {
        if (_playerGraphics.sprite == _data.WeakSprite || _playerGraphics.sprite == _data.StrongSprite)
            _playerGraphics.sprite = _isWeak ? _data.WeakAttackingSprite : _data.StrongAttackingSprite;
    }
    private void DoBiteAnimation()
    {
        if (_playerGraphics.sprite == _data.WeakAttackingSprite || _playerGraphics.sprite == _data.StrongAttackingSprite)
            _playerGraphics.sprite = _isWeak ? _data.WeakEatingAnimation[0] : _data.StrongEatingAnimation[0];
    }
    private void DoEatingAnimation()
    {
        if (_playerGraphics.sprite == _data.WeakEatingAnimation[0] || _playerGraphics.sprite == _data.StrongEatingAnimation[0])
            _playerGraphics.sprite = _isWeak ? _data.WeakEatingAnimation[1] : _data.StrongEatingAnimation[1];
        else
            _playerGraphics.sprite = _isWeak ? _data.WeakEatingAnimation[0] : _data.StrongEatingAnimation[0];
    }

    public void ChangeState(PlayerStates newState)
    {
        switch (newState)
        {
            case PlayerStates.Idle:
                _playerState = Idle;
                break;
            case PlayerStates.Moving:
                _playerState = Moving;
                break;
            case PlayerStates.Attacking:
                _playerState = Attacking;
                break;
            case PlayerStates.Biting:
                _playerState = Biting;
                break;
            case PlayerStates.Eating:
                _playerState = Eating;
                break;
            case PlayerStates.FailedBiting:
                _playerState = FailedBiting;
                break;
            case PlayerStates.Casting:
                _playerState = Casting;
                break;
        }
    }

    private void InvokeDeath()
    {
        OnDeath?.Invoke();
    }
    public virtual void Die()
    {
        InvokeDeath();
    }
    public bool TakeDamage(int damage)
    {
        bool isAlive;
        _damageTaken += damage;

        UIManager.Instance.UpdateHearts();

        //Debug.Log($"Player.TakeDamage({damage}). Hp is now {Hp}");
        if (Hp <= 0)
        {
            isAlive = true;
            Die();
            return isAlive;
        }

        isAlive = false;
        return isAlive;
    }

    public bool UseMana()  // Added for spellcasting
    {
        if (mana > 10)
        {
            mana -= 10;
            UIManager.Instance.UpdateMana(mana);
            return true;
        }

        return false;
    }



    private void OnDrawGizmos()
    {
        // cyan = biteRange
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, _data.BiteDistance * Vector2.right);
    }
}
