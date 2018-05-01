using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapElement : JrpgBehaviour
{

	// Use this for initialization
	public virtual void Start ()
    {
		
	}
	
	// Update is called once per frame
	public virtual void Update ()
    {
        Jrpg.AdjustSortingOrder(gameObject);
    }
}
