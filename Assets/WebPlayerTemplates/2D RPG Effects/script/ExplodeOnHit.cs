using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnHit : MonoBehaviour {

    public GameObject explosion;
    public GameObject Spawner;

    // Use this for initialization
    void Start () {
        Instantiate(Spawner, transform.position, transform.rotation);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<Collider>().gameObject.tag == "Wall")
        {
            Instantiate(explosion , transform.position, transform.rotation);
            Destroy(gameObject,0.15f);
        }
    }
}
