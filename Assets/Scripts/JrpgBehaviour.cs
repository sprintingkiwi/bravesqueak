using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JrpgBehaviour : MonoBehaviour
{
    // FUNCTION CALLED BY TRIGGER SENSORS
    public virtual void TriggerSensor(string sensor, Collider2D other, string type)
    {
        Debug.Log("Triggered sensor " + name + " by " + other.name + " type: " + type);
    }

    // FUNCTION CALLED BY COLLISION SENSORS 
    public virtual void CollisionSensor(string sensor, Collision2D collision, string type)
    {

    }
}
