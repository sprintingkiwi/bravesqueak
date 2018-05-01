using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skit : AnimatedMapElement
{
    public Encounter encounter;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        Jrpg.AdjustSortingOrder(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Jrpg.Log("Triggered skit " + name);

        // Trigger battle when player is in range
        if (other.gameObject.name == "Player")
        {
            StartCoroutine(TriggerBattle());
        }
    }

    IEnumerator TriggerBattle()
    {
        gc.player.canMove = false;

        // Focus camera to skit
        gc.mapCamera.followPlayer = false;
        //yield return StartCoroutine(mapCam.Move(Vector3.up * 5));
        yield return StartCoroutine(gc.mapCamera.MoveTo(new Vector3(spr.bounds.center.x, spr.bounds.center.y - 1.3f, gc.mapCamera.transform.position.z), speed: 0.5f));

        // If the skit has a prebattle animation, then play it
        foreach (AnimatorControllerParameter acp in anim.parameters)
            if (acp.name == "prebattle")
            {
                anim.SetTrigger("prebattle");
                break;
            }
        
        yield return new WaitForSeconds(5f);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);

        // Trigger battle
        gc.StartCoroutine(gc.TriggerBattle(encounter, name));
    }
}
