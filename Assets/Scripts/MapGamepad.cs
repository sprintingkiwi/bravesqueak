using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGamepad : MonoBehaviour
{

	void Awake ()
    {
        if (Jrpg.CheckPlatform() == "Mobile")
            foreach (Transform t in transform)
                t.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
