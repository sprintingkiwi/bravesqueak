using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchscreenOnly : MonoBehaviour
{    void Start()
    {
        gameObject.SetActive(GameController.Instance.touchControls);
    }
}
