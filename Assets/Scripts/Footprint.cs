using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprint : MonoBehaviour
{
    public float lifeTime;
    float startTime;
    public SpriteRenderer ownerSR;
    SpriteRenderer printSR;

	// Use this for initialization
	void Start ()
    {
        startTime = Time.time;

        printSR = transform.Find("Print").gameObject.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (ownerSR != null)
            printSR.sortingOrder = ownerSR.sortingOrder - 1;

		if (Time.time - startTime > lifeTime)
        {
            Destroy(gameObject);
        }
	}
}
