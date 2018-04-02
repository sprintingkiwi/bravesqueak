using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public Battler holder;

	// Use this for initialization
	public void Setup ()
    {
        holder = transform.parent.parent.GetComponent<Battler>();
        Debug.Log(holder.name + " is " + name);
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
