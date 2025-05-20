using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
    public float turnSpeed;
    public IKLimb ikl;

    void Update()
    {
        
    }

    public void Movement()
    {
        direction.Normalize();
        if (true /*Input.GetKey(KeyCode.W)*/)
        {
            transform.position += direction * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction = Quaternion.Euler(0f, 0f, turnSpeed * Time.deltaTime) * direction;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction = Quaternion.Euler(0f, 0f, -turnSpeed * Time.deltaTime) * direction;
        }
    }

    private void FixedUpdate()
    {
        ikl.FABRIK(transform.position + direction);
    }

    private void Start()
    {
        ikl.independent = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }
}
