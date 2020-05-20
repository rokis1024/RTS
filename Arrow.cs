using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    GameObject target;
    Unit shooter;
    private float speed = 55f;

    public void SetTarget(GameObject unitTarget, Unit unit)
    {
        target = unitTarget;
        shooter = unit;
    }

    // Update is called once per frame
    private void Update()
    {
        if (target != null)
        {
            Vector3 dir = target.GetComponent<ObjectInfo>().strikePosition.transform.position - transform.position;
            float distanceThisFrame = speed * Time.deltaTime;

            if (dir.magnitude <= distanceThisFrame)
            {
                if (shooter != null)
                {
                    shooter.Attack();
                }

                Destroy(gameObject);
                return;
            }

            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        }
        else
        {
            Destroy(gameObject);

            return;
        }
    }
}
