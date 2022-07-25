using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenu : MonoBehaviour
{
    [Header("Drag and Drop")]
    public Transform buttonsContainer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.ButtonBDown())
        {
            if (buttonsContainer.gameObject.activeSelf)
                buttonsContainer.gameObject.SetActive(false);
            else
                buttonsContainer.gameObject.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
