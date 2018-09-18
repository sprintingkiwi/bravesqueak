using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MenuButton
{
    public void StartNewGame()
    {
        StartCoroutine(Jrpg.JumpAway(GameObject.Find("Title"), Vector3.up));
        StartCoroutine(Jrpg.JumpAway(gameObject, Vector3.down, power: 20f));
        StartCoroutine(Jrpg.LoadScene("World"));
    }
}
