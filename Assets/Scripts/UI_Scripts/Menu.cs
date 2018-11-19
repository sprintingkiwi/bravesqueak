﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameController gc;
    public InputManager inputManager;

    // Use this for initialization
    public virtual void Setup ()
    {
        Jrpg.Log("Starting menu: " + name);
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
    }
	
	// Update is called once per frame
	public virtual void Update ()
    {
		if (Input.GetButton("Cancel"))
        {
            // Delete menu-related stuff under canvas
            foreach (Transform t in GameObject.Find("Canvas").transform)
                if (t.name.Contains(name))
                    Destroy(t.gameObject);

            gc.currentMap.gameObject.SetActive(true);
            Destroy(this.gameObject);
            gc.player.canMove = true;
        }
	}
}