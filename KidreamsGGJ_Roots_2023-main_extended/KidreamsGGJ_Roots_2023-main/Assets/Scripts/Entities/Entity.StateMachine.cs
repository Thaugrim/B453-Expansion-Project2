using System;
using UnityEngine;

public partial class Entity
{
    [SerializeField] private bool _entityDebug = false;

    private EntityState State
    {
        get => _state;
        set
        {
            if (value == _state) return;

            _updateAction = value switch
            {
                EntityState.Idle => UpdateIdleState,
                EntityState.ChasingPlayer => UpdateChasingState,
                EntityState.RunningFromPlayer => UpdateRunningAwayState,
                EntityState.Attacking => UpdateAttackingState,
                EntityState.CapturedByPlayer => UpdateCapturedState,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            
            Action<EntityState> transitionAction = value switch
            {
                EntityState.Idle => TransitionToIdle,
                EntityState.ChasingPlayer => TransitionToChasing,
                EntityState.RunningFromPlayer => TransitionToRunning,
                EntityState.Attacking => TransitionToAttacking,
                EntityState.CapturedByPlayer => TransitionToCaptured,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };

            transitionAction(_state);
            _state = value;
        }
    }

    protected virtual void TransitionToIdle(EntityState prevState)
    {
        if (_entityDebug)
            Debug.Log(LogStr(nameof(TransitionToIdle)), gameObject);
        
        _navigation.enabled = true;
        _navigation.SetState(CachedPlayerTransform, EntityNavigation.NavigationMode.MoveRandomly);
        _anim.SetTrigger(AnimTrigger_Idle);
        moveStaggerAnim.enabled = true;
    }

    protected virtual void TransitionToChasing(EntityState prevState)
    {
        if (_entityDebug)
            Debug.Log(LogStr(nameof(TransitionToChasing)));
        
        _navigation.enabled = true;
        _navigation.SetState(CachedPlayerTransform, EntityNavigation.NavigationMode.MoveToPlayer);
        
        _anim.SetTrigger(AnimTrigger_ChasingPlayer);
        moveStaggerAnim.enabled = true;
    }

    protected virtual void TransitionToRunning(EntityState prevState)
    {
        if (_entityDebug)
            Debug.Log(LogStr(nameof(TransitionToRunning)));
        
        _navigation.enabled = true;
        _navigation.SetState(CachedPlayerTransform, EntityNavigation.NavigationMode.RunFromPlayer);
        
        moveStaggerAnim.enabled = true;
        _anim.SetTrigger(AnimTrigger_RunningFromPlayer);
    }

    protected virtual void TransitionToAttacking(EntityState prevState)
    {
        if (_entityDebug)
            Debug.Log(LogStr(nameof(TransitionToAttacking)));
        
        _navigation.enabled = false;
        
        moveStaggerAnim.enabled = false;
        _anim.SetTrigger(AnimTrigger_Attack);
    }

    private float _lastAttackTime;
    
    
    protected virtual void TransitionToCaptured(EntityState prevState)
    {
        if (_entityDebug)
            Debug.Log(LogStr(nameof(TransitionToCaptured)));
        _navigation.enabled = false;
        
        moveStaggerAnim.enabled = false;
        _anim.SetTrigger(AnimTrigger_Idle);
    }
    
    protected virtual void UpdateIdleState()
    {
        if (_playerInSight)
        {
            State = PlayerSeenState;
        }
    }
    
    protected virtual void UpdateChasingState()
    {
        if (!_playerInSight)
        {
            State = EntityState.Idle;
            return;
        }
        var isInAttackRange = IsPlayerInAttackRange();
        if (isInAttackRange) State = EntityState.Attacking;
    }

    protected virtual void UpdateRunningAwayState()
    {
        // Don't go idle if dont see player ( youre running away from him)
    }
    
    protected virtual void UpdateAttackingState()
    {
        if (!_playerInSight)
        {
            State = EntityState.Idle;
            return;
        }

        if (!IsPlayerInAttackRange())
        {
            State = PlayerSeenState;
        }

        //if (State == EntityState.CapturedByPlayer)
        //{
        //    return;
        //}

        if (Time.time - _lastAttackTime > Data.CommonData.EntityAttackTimeDelta)
        {
            _cachedPlayer.TakeDamage(Data.Damage);
            _lastAttackTime = Time.time;
        }
    }
    
    protected virtual void UpdateCapturedState()
    {
        
    }


    protected void OnNavigationReachedDestination() // Currently only for random pos / runaway (not reached player)
    {
        if (State == EntityState.RunningFromPlayer && !_playerInSight)
        {
            State = EntityState.Idle;
            return;
        }
    }
}