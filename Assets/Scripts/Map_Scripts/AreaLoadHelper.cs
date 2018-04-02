using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AreaLoadHelper : MonoBehaviour
{
    GameController gc;   

    // Update is called once per frame
    void Start ()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        gc.InitializeGame();
    }
}
