using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AreaLoadHelper : MonoBehaviour
{
    // Update is called once per frame
    void Start ()
    {
        GameController.Instance.InitializeGame();
    }
}
