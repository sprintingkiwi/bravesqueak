using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skit : MonoBehaviour
{
    public Encounter encounter;
    GameController gc;
    Animator anim;

    void Start()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        anim = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
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
        // If the skit has a prebattle animation, then play it
        foreach (AnimatorControllerParameter acp in anim.parameters)
            if (acp.name == "prebattle")
            {
                anim.SetTrigger("prebattle");
                break;
            }

        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);

        // Trigger battle
        gc.StartCoroutine(gc.TriggerBattle(encounter, name));
    }
}
