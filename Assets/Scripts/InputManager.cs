using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [Header("Swipe")]
    public bool swiping = false;
    public bool eventSent = false;
    public Vector2 lastPosition;
    public string swipe;

    [Header("UI Buttons")]
    public string touchButtonInput;

    [Header("Other")]
    float hor;
    float ver;
    bool horAxisPressed;
    bool verAxisPressed;

    // Persistence
    public static InputManager cGInstance;
    void Awake()
    {
        // Persistence
        if (cGInstance == null)
        {
            cGInstance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {
        swipe = SwipeCheck();

        hor = Input.GetAxis("Horizontal");
        if (-0.1f <= hor && hor <= 0.1f)
            horAxisPressed = false;
        ver = Input.GetAxis("Vertical");
        if (-0.1f <= ver && ver <= 0.1f)
            verAxisPressed = false;
    }

    // BUTTON A
    public bool ButtonADown()
    {
        if (Input.GetButtonDown("ButtonA"))
            return true;
        else if (touchButtonInput == "ButtonA Down")
        {
            touchButtonInput = "None";
            Debug.Log("Button A UI touch input received");
            return true;
        }
        //else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        //    return true;
        else
            return false;
    }
    public bool ButtonAUp()
    {
        if (Input.GetButtonUp("ButtonA"))
            return true;
        else if (touchButtonInput == "ButtonA Up")
        {
            touchButtonInput = "None";
            Debug.Log("ButtonA button UI touch input received");
            return true;
        }
        else
            return false;
    }

    //BUTTON B
    public bool ButtonBDown()
    {
        if (Input.GetButtonDown("ButtonB"))
            return true;
        else if (touchButtonInput == "ButtonB Down")
        {
            touchButtonInput = "None";
            Debug.Log("Button B UI touch input received");
            return true;
        }
        //else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        //    return true;
        else
            return false;
    }
    public bool ButtonBUp()
    {
        if (Input.GetButtonUp("ButtonB"))
            return true;
        else if (touchButtonInput == "ButtonB Up")
        {
            touchButtonInput = "None";
            Debug.Log("ButtonB button UI touch input received");
            return true;
        }
        else
            return false;
    }

    // RIGHT
    public bool RightArrow()
    {
        if (Input.GetButton("Right"))
            return true;
        else if (Input.GetAxis("Horizontal") > 0)
            return true;
        else if (touchButtonInput == "Right Down")
        {
            Debug.Log("Right button UI touch input received");
            return true;
        }
        else
            return false;
    }
    public bool RightArrowDown()
    {
        if (Input.GetButtonDown("Right"))
            return true;
        else if (touchButtonInput == "Right Down")
        {
            touchButtonInput = "None";
            Debug.Log("Right button UI touch input received");
            return true;
        }
        else if (hor > 0.1f && !horAxisPressed)
        {
            horAxisPressed = true;
            return true;
        }
        else
            return false;
    }
    public bool RightArrowUp()
    {
        if (Input.GetButtonUp("Right"))
            return true;
        else if (touchButtonInput == "Right Up")
        {
            touchButtonInput = "None";
            Debug.Log("Right button UI touch input received");
            return true;
        }
        else
            return false;
    }

    // LEFT
    public bool LeftArrow()
    {
        if (Input.GetButton("Left"))
            return true;
        else if (Input.GetAxis("Horizontal") < 0)
            return true;
        else if (touchButtonInput == "Left Down")
        {
            Debug.Log("Left button UI touch input received");
            return true;
        }
        else
            return false;
    }
    public bool LeftArrowDown()
    {
        if (Input.GetButtonDown("Left"))
            return true;
        else if (touchButtonInput == "Left Down")
        {
            touchButtonInput = "None";
            Debug.Log("Left Down button UI touch input received");
            return true;
        }
        else if (hor < -0.1f && !horAxisPressed)
        {
            horAxisPressed = true;
            return true;
        }
        else
            return false;
    }
    public bool LeftArrowUp()
    {
        if (Input.GetButtonUp("Left"))
            return true;
        else if (touchButtonInput == "Left Up")
        {
            touchButtonInput = "None";
            Debug.Log("Left Up button UI touch input received");
            return true;
        }
        else
            return false;
    }

    // DOWN
    public bool DownArrow()
    {
        if (Input.GetButton("Down"))
            return true;
        else if (Input.GetAxis("Vertical") < 0)
            return true;
        else if (touchButtonInput == "Down Down")
        {
            Debug.Log("Down button UI touch input received");
            return true;
        }
        else
            return false;
    }
    public bool DownArrowDown()
    {
        if (Input.GetButtonDown("Down"))
            return true;
        else if (touchButtonInput == "Down Down")
        {
            touchButtonInput = "None";
            Debug.Log("Down Down button UI touch input received");
            return true;
        }
        else if (ver < -0.1f && !verAxisPressed)
        {
            verAxisPressed = true;
            return true;
        }
        else
            return false;
    }
    public bool DownArrowUp()
    {
        if (Input.GetButtonUp("Down"))
        {
            return true;
        }
        else if (touchButtonInput == "Down Up")
        {
            touchButtonInput = "None";
            Debug.Log("Down Up button UI touch input received");
            return true;
        }
        else
            return false;
    }

    // UP
    public bool UpArrow()
    {
        if (Input.GetButton("Up"))
            return true;
        else if (Input.GetAxis("Vertical") > 0)
            return true;
        else if (touchButtonInput == "Up Down")
        {
            Debug.Log("Down button UI touch input received");
            return true;
        }
        else
            return false;
    }
    public bool UpArrowDown()
    {
        if (Input.GetButtonDown("Up"))
            return true;
        else if (touchButtonInput == "Up Down")
        {
            touchButtonInput = "None";
            Debug.Log("Up Down button UI touch input received");
            return true;
        }
        else if (ver > 0.1f && !verAxisPressed)
        {
            verAxisPressed = true;
            return true;
        }
        else
            return false;
    }
    public bool UpArrowUp()
    {
        if (Input.GetButtonUp("Up"))
        {
            return true;
        }
        else if (touchButtonInput == "Up Up")
        {
            touchButtonInput = "None";
            Debug.Log("Up Up button UI touch input received");
            return true;
        }
        else
            return false;
    }

    //void RightButtonTask()
    //{
    //    Debug.Log("You have clicked the right button!");
    //}

    //void LeftButtonTask()
    //{
    //    Debug.Log("You have clicked the left button!");
    //}

    public void ProcessButtonTouch(string button, string phase)
    {
        touchButtonInput = button + " " + phase;
    }    

    string SwipeCheck()
    {
        if (Input.touchCount == 0)
            return "None";

        if (Input.GetTouch(0).deltaPosition.sqrMagnitude != 0)
        {
            if (!swiping)
            {
                swiping = true;
                lastPosition = Input.GetTouch(0).position;
                return "None";
            }
            else
            {
                Vector2 direction = Input.GetTouch(0).position - lastPosition;

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    if (direction.x > 0)
                        return "Right";
                    else
                        return "Left";
                }
                else
                {
                    if (direction.y > 0)
                        return "Up";
                    else
                        return "Down";
                }
            }
        }
        else
        {
            swiping = false;
        }

        return "None";
    }

}
