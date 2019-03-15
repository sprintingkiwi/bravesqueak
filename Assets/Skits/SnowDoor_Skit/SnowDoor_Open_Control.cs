using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDoor_Open_Control : Transfer {

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.parent.parent.Find("ENEMIES").childCount > 0)
        {
            Jrpg.Log("Still not beaten the mini bosses!");
            return;
        }

        gameObject.GetComponent<AnimatedMapElement>().anim.SetTrigger("prebattle");

        base.OnTriggerEnter2D(collision);
    }
}
