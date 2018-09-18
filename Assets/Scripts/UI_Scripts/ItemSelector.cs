using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelector : MonoBehaviour
{
    Item selectedItem;
    string pool;

    [Header("System")]
    HeroMenu hm;

    // Use this for initialization
    void Start ()
    {
        hm = transform.parent.parent.GetComponent<HeroMenu>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnMouseDown()
    {
        hm.ChangeItem(this, pool);
    }
}
