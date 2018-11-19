using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [TextArea(5, 10)]
    public string description;

    // Use this for initialization
    public virtual void Start ()
    {
		
	}

    // Update is called once per frame
    public virtual void Update()
    {
		
	}

    public virtual IEnumerator Fall()
    {
        // Food fall down code


        yield return null;
    }
}
