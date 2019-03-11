using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skit : AnimatedMapElement
{
    [Header("Skit")]
    public Encounter encounter;
    public AnimationClip preBattleClip;

    [Header("Camera Shift")]
    public float shiftThresold = 1f;

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
            StartCoroutine(ProcessSkitEncounter());
        }
    }

    IEnumerator ProcessSkitEncounter()
    {
        gc.player.canMove = false;

        // Focus camera to skit
        gc.mapCamera.followPlayer = false;
        //yield return StartCoroutine(mapCam.Move(Vector3.up * 5));
        gc.mapCamera.activeCoroutines.Add(StartCoroutine(gc.mapCamera.SlideTo(new Vector3(spr.bounds.center.x, spr.bounds.center.y - 1.3f, gc.mapCamera.transform.position.z), threshold: shiftThresold, speed: 0.5f, callback: TriggerBattle())));

        yield return null;
    }

    public virtual IEnumerator TriggerBattle()
    {
        // If the skit has a prebattle animation, then play it
        foreach (AnimatorControllerParameter acp in anim.parameters)
            if (acp.name == "prebattle")
            {
                anim.SetTrigger("prebattle");

                // Wait for pre-battle animation
                //TODO: it should be improved by automatically check clip lenght from mecanim
                if (preBattleClip != null)
                    yield return new WaitForSeconds(preBattleClip.length);
                else
                {
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
                }

                break;
            }

        // Trigger Battle
        yield return StartCoroutine(gc.TriggerBattle(encounter, name));
    }
}
