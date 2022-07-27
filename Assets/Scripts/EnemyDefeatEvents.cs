using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDefeatEvents : MonoBehaviour
{
    public Skit[] enemySkits;
    public UnityEvent[] defeatEvents;

    bool triggerEvents;

    // Update is called once per frame
    void Update()
    {
        triggerEvents = true;
        foreach (Skit skit in enemySkits)
            if (skit != null) triggerEvents = false;

        if (triggerEvents) foreach (UnityEvent de in defeatEvents) de.Invoke();
    }
}
