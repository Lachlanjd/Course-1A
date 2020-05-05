using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SecondaryFire : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private float _speed = 8f;
    [SerializeField]
    private float _rotateSpeed = 250f;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _target = GameObject.FindGameObjectWithTag("Enemy").transform;    
    }

    private void Update()
    {
        if (_target == null)
        {
            _target = GameObject.FindGameObjectWithTag("Enemy").transform;
        }

        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
        if (transform.position.y > 8f || transform.position.y < -8f)
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 direction = (Vector2)_target.position - rb.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * _rotateSpeed;
        rb.velocity = transform.up * _speed;
    }
}
