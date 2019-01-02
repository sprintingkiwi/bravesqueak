using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public Battler holder;

	// Use this for initialization
	public void Setup ()
    {
        //holder = transform.parent.parent.gameObject.GetComponent<Battler>();
        holder = gameObject.GetComponentInParent<Battler>();
        Jrpg.Log(holder.name + " is " + name, "Visible");
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public virtual void Effect()
    {
        Debug.Log("Processing " + gameObject.name + " effect on " + holder.name);
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
