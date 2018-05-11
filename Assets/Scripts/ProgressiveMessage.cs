using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressiveMessage : System.Object
{
    public string message;
    public float charPause = 0.1f;
    public float afterPause = 1f;
    public bool fadeAfter;
}
