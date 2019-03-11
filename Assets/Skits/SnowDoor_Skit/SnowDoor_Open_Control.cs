using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDoor_Open_Control : Skit {

    public override IEnumerator TriggerBattle()
    {
        if (GameObject.Find("Yeti_Skit") != null || GameObject.Find("Snowman_Skit") != null)
        {
            Jrpg.Log("Still not beaten the mini bosses!");
            yield break;
        }

        yield return StartCoroutine(base.TriggerBattle());
    }
}
