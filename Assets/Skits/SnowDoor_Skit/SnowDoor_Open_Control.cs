using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDoor_Open_Control : Transfer {

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if both mini bosses are dead
        if (transform.parent.parent.Find("ENEMIES").childCount > 0)
        {
            Jrpg.Log("Still not beaten the mini bosses!");
            return;
        }

        // Start door animation with trigger
        gameObject.GetComponent<AnimatedMapElement>().anim.SetTrigger("prebattle");

        // Wait animation then transfer coroutine
        StartCoroutine(WaitAndOpen(collision));
    }

    IEnumerator WaitAndOpen(Collider2D collision)
    {
        yield return new WaitForSeconds(4f);
        base.OnTriggerEnter2D(collision);
        yield return null;
    }
}
