using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    GameController gc;
    InputManager inputManager;

    // Use this for initialization
    public virtual void Start ()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
