using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour
{
    public UnityEvent buttonEvent;
    public string alternativeButton;

    [Header("Pulse")]
    // Animate the game object from -1 to +1 and back
    public float minimum = -1.0F;
    public float maximum = 1.0F;
    // Starting value for the Lerp
    static float t = 0.0f;
    // Lerp options
    public float speed;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        Pulse();

        if (alternativeButton != "")
        {
            if (Input.GetButtonDown(alternativeButton))
            {
                StartNewGame();
            }
        }
	}

    void OnMouseDown()
    {        
        StartNewGame();
    }

    public void StartNewGame()
    {
        StartCoroutine(Jrpg.JumpAway(GameObject.Find("Title"), Vector3.up));
        StartCoroutine(Jrpg.JumpAway(gameObject, Vector3.down, power: 20f));
        StartCoroutine(Jrpg.LoadScene("World"));
    }

    void Pulse()
    {
        // animate the position of the game object...
        transform.localScale = Mathf.Lerp(minimum, maximum, t) * Vector3.one;

        // .. and increate the t interpolater
        t += speed * Time.deltaTime;

        // now check if the interpolator has reached 1.0
        // and swap maximum and minimum so game object moves
        // in the opposite direction.
        if (t > 1.0f)
        {
            float temp = maximum;
            maximum = minimum;
            minimum = temp;
            t = 0.0f;
        }
    }
}
