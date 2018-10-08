using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Patrol : EnemyCharacter
{
    public Transform patrolPoints;
    public Vector3[] destinations;
    public int nextPlace;

    public override void Start()
    {
        base.Start();

        destinations = new Vector3[patrolPoints.childCount];
        foreach (Transform p in patrolPoints)
            destinations[p.GetSiblingIndex()] = p.position;

        BeginPatrol();
    }

    public override void Update()
    {
        base.Update();

        PatrolMovement();
    }

    void PatrolMovement()
    {
        if ((transform.position - destinations[nextPlace]).magnitude > 0.1f)
        {
            Move((destinations[nextPlace] - transform.position).normalized);
        }
        else
        {
            if (nextPlace < destinations.Length - 1)
            {
                nextPlace += 1;
            }
            else
            {
                nextPlace = 0;
            }
        }
    }

    void BeginPatrol()
    {
        transform.position = destinations[0];
        nextPlace = 1;
    }
}
