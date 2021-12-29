using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public Effect statusEffect;
    public float decadence;
    float lastSaveRollThreshold = 0;

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
        
        // Process status effect and wait for its completion only if it's not in loop mode
        yield return StartCoroutine(Jrpg.PlayEffect(holder, statusEffect, true));
        //yield return StartCoroutine(Jrpg.PlayEffect(holder, statusEffect, !statusEffect.loop));
    }

    public virtual IEnumerator Effect()
    {
        Debug.Log("Processing " + gameObject.name + " effect on " + holder.name);
        
        // Repeat feedback if status effect is not in loop mode
        //if (!statusEffect.loop)
        yield return StartCoroutine(Feedback());
        //else // else just play the hit animation
        //    yield return StartCoroutine(Jrpg.PlayAnimation(holder, "hit", true));

        yield return null;
    }

    public virtual IEnumerator SaveRoll()
    {
        Debug.Log("Save roll for " + holder.name + " to remove " + gameObject.name);

        // Check for status removal
        if (Random.Range(0, 100) < lastSaveRollThreshold)
        {
            yield return StartCoroutine(RemoveStatus());
        }

        // Add decadence to increase removal chances every turn
        lastSaveRollThreshold += decadence;

        yield return null;
    }

    public virtual void CheckDestroyed()
    {
        Debug.Log("Checking for " + gameObject.name + " destroy condition");
    }

    public virtual IEnumerator RemoveStatus()
    {
        Debug.Log("Removing " + gameObject.name + " of " + holder.name);
        this.enabled = false; // will be destroyed in Battler.ProcessStatusEffects

        yield return null;
    }
}
