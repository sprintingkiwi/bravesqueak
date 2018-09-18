using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{
    InputManager inputManager;
    SpriteRenderer spr;
    public Sprite up;
    public Sprite down;
    public float fadeSpeed = 0.2f;
    public float alpha = 1;

	// Use this for initialization
	public void Start ()
    {
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
        spr = gameObject.GetComponent<SpriteRenderer>();
        spr.sprite = up;

        // Fade in
        //Jrpg.Fade(gameObject, alpha, fadeSpeed);
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1);
    }

    // Update is called once per frame
    public void Update ()
    {

	}

    void OnMouseDown()
    {
        Debug.Log(name + " touched down");

        spr.sprite = down;
        inputManager.ProcessButtonTouch(name, "Down");
    }

    void OnMouseUp()
    {
        Debug.Log(name + " touched up");

        spr.sprite = up;
        inputManager.ProcessButtonTouch(name, "Up");
    }
}
