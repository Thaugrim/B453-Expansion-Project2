using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Entity
{
    [SerializeField] private SpriteRenderer _villigerGraphics;
    [SerializeField] private float _tombInstansiationOffsetY = 10f, _tombOffsetY = 3.25f;

    // TODO: projectiles (handle attack range in base class first?)
    // TODO: Grave system (separate class, but will hold the Villagers

    protected override void TransitionToAttacking(EntityState prevState)
    {
        base.TransitionToAttacking(prevState);
    }

    protected override void UpdateAttackingState()
    {
        base.UpdateAttackingState();
    }
}
