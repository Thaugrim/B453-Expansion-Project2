using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _maxDistance;
    

    Vector3 originalPos, target;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPos = transform.position;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.up * _speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision != null)
            Destroy(gameObject);
    }
}
