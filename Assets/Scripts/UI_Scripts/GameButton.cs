using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButton : MonoBehaviour
{
    SpriteRenderer spr;
    public Sprite up;
    public Sprite down;
    public float fadeSpeed = 0.2f;
    public float alpha = 1;
    bool physicalButtonFeedback;

	// Use this for initialization
	public void Start ()
    {
        InputManager.instance = GameObject.Find("Input Manager").GetComponent<InputManager>();
        spr = gameObject.GetComponent<SpriteRenderer>();
        spr.sprite = up;

        // Fade in
        //Jrpg.Fade(gameObject, alpha, fadeSpeed);
        //spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1);

        //physicalButtonFeedback = IsButtonAvailable(name);
    }

    // Update is called once per frame
    public void Update ()
    {
        //if (physicalButtonFeedback)
        //{
        //    if (Input.GetButtonDown(name))
        //        spr.sprite = down;
        //    else if (Input.GetButtonUp(name))
        //        spr.sprite = up;
        //}        
	}

    bool IsButtonAvailable(string btnName)
    {
        try
        {
            Input.GetButton(btnName);
            return true;
        }
        catch (UnityException exc)
        {
            return false;
        }
    }

    void OnMouseDown()
    {
        Debug.Log(name + " touched down");

        spr.sprite = down;
        InputManager.instance.ProcessButtonTouch(name, "Down");
    }

    void OnMouseUp()
    {
        Debug.Log(name + " touched up");

        spr.sprite = up;
        InputManager.instance.ProcessButtonTouch(name, "Up");
    }
}
