using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtonUI : MonoBehaviour
{
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

    public void ButtonDown()
    {
        Debug.Log(name + " touched down");
        InputManager.instance.ProcessButtonTouch(name, "Down");
    }

    //public void ButtonUp()
    //{
    //    Debug.Log(name + " touched up");
    //    InputManager.instance.ProcessButtonTouch(name, "Up");
    //}
}
