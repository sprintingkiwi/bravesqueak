﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAccessibility : MonoBehaviour
{
    Font lastFont;

    // Start is called before the first frame update
    void Start()
    {
        lastFont = gameObject.GetComponent<Text>().font;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastFont != GameController.Instance.activeFont)
        {
            gameObject.GetComponent<Text>().font = GameController.Instance.activeFont;
            lastFont = GameController.Instance.activeFont;
        }
    }
}
