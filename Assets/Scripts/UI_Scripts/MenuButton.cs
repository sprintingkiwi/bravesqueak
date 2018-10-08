using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour
{
    public UnityEvent buttonEvent;
    public string alternativeButton;
    public bool pulse;
    public bool multiInput;

    [Header("Pulse")]
    // Animate the game object from -1 to +1 and back
    public float minimum = -1.0F;
    public float maximum = 1.0F;
    // Starting value for the Lerp
    public float t = 0.0f;
    // Lerp options
    public float speed;

    // Use this for initialization
    public virtual void Start ()
    {
		
	}
	
	// Update is called once per frame
	public virtual void Update ()
    {
        if (pulse)
            Pulse();

        if (alternativeButton != "")
        {
            if (Input.GetButtonDown(alternativeButton))
            {
                buttonEvent.Invoke();
            }
        }
	}

    public virtual void OnMouseDown()
    {
        buttonEvent.Invoke();

        if (!multiInput)
            gameObject.GetComponent<Collider2D>().enabled = false;
    }

    public virtual void Pulse()
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
