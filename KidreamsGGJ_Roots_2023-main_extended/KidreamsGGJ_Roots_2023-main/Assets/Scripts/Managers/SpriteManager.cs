using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    private static SpriteManager _instance;
    public static SpriteManager Instance;

    private void Awake()
    {
        _instance = this;
    }

    public Vector2 FlipSpriteCorrectly(Vector2 moveDirection, Transform transformToFlip)
    {
        Vector2 newDirection = Vector2.zero;

        if (moveDirection.x < 0)
        {
            newDirection = new(-Mathf.Abs(transformToFlip.localScale.x), 0);
            transformToFlip.localScale = new Vector3(-Mathf.Abs(transformToFlip.localScale.x), transformToFlip.localScale.y, transformToFlip.localScale.z);
        }
        else if (moveDirection.x > 0)
        {
            newDirection = new(Mathf.Abs(transformToFlip.localScale.x), 0);
            transformToFlip.localScale = new Vector3(Mathf.Abs(transformToFlip.localScale.x), transformToFlip.localScale.y, transformToFlip.localScale.z);
        }

        return newDirection;
    }
}
