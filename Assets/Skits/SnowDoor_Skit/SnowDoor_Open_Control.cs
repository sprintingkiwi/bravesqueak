using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowDoor_Open_Control : MonoBehaviour {

    [Header("Drag and Drop")]
    public Skit[] miniBosses;
    public GameObject returnHook;

    bool openDoor;

    void Update()
    {
        // Check if both mini bosses are dead
        openDoor = true;
        foreach (Skit skit in miniBosses) if (skit != null) openDoor = false;

        // Show transfer indicator
        returnHook.SetActive(openDoor);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!openDoor)
        {
            Jrpg.Log("Still not beaten the mini bosses!");
            return;
        }

        // Start door animation with trigger
        gameObject.GetComponent<AnimatedMapElement>().anim.SetTrigger("prebattle");
    }
}
