using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loading : MonoBehaviour
{
    // Not working yet
    public void Execute()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        Jrpg.Fade(gameObject, 1, 0.2f);
    }
}
