using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    // Art https://opengameart.org/content/cartoon-fireball-1600x1600-png

    [SerializeField] private float _speed;
    
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.AddForce(transform.up * _speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.gameObject.layer == LayerMask.NameToLayer("Animals"))
        {
            collision.transform.root.gameObject.SetActive(false);
            Debug.Log("Hit Animal!");
        }

        if (collision != null)
            Destroy(gameObject);

    }
}
