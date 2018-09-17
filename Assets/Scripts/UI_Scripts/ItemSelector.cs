using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelector : MonoBehaviour
{
    Item selectedItem;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnMouseDown()
    {
        Jrpg.Log("Intantiating Item Selection Menu");
        GameObject selMenu = Instantiate(Resources.Load("Menu/ItemSelectionMenu") as GameObject);

    }
}
