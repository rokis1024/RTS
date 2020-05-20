using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StoneGate : MonoBehaviour
{
    public bool open = false;
    private NavMeshObstacle obstacle;
    private float speed = 15f;
    private Vector3 newPos;
    private Vector3 original;

    void Start()
    {
        obstacle = GetComponent<NavMeshObstacle>();
        original = this.transform.position;
    }
    
    void Update()
    {
        if(open)
        {
            newPos = new Vector3(transform.position.x, transform.position.y - 19f, transform.position.z);
            obstacle.enabled = false;
            if(newPos.y <= transform.position.y)
            {
                transform.position -= transform.up * Time.deltaTime * speed;
            }
        }
        else
        {
            newPos = original;
            obstacle.enabled = true;
            if (newPos.y >= transform.position.y)
            {
                transform.position += transform.up * Time.deltaTime * speed;
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.layer == 8)
        {
            open = true;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.layer == 8)
        {
            open = false;
        }
    }
}
