using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tussock : MonoBehaviour
{
    GameObject grassBlade;

	// Use this for initialization
	void Start ()
    {
        grassBlade = transform.Find("GrassBlade").gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
