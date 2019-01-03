using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public Effect statusEffect;

    [Header("System")]
    public Battler holder;

    // Use this for initialization
    public virtual IEnumerator Setup ()
    {
        //holder = transform.parent.parent.gameObject.GetComponent<Battler>();
        holder = gameObject.GetComponentInParent<Battler>();
        Jrpg.Log(holder.name + " is " + name, "Visible");

        yield return StartCoroutine(Feedback());
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public virtual IEnumerator Feedback()
    {
        // Status entry effects here
        yield return StartCoroutine(Jrpg.PlayAnimation(holder, "hit", true));
        yield return StartCoroutine(Jrpg.PlayEffect(holder, statusEffect, true));
    }

    public virtual IEnumerator Effect()
    {
        Debug.Log("Processing " + gameObject.name + " effect on " + holder.name);
        yield return StartCoroutine(Feedback());
    }

    public virtual void SaveRoll()
    {
        Debug.Log("Save roll for " + holder.name + " to remove " + gameObject.name);
    }

    public virtual void CheckDestroyed()
    {
        Debug.Log("Checking for " + gameObject.name + " destroy condition");
    }
}
