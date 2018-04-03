using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skit : MonoBehaviour
{
    public Encounter encounter;
    GameController gc;

    void Start()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    private void Update()
    {
        Jrpg.AdjustSortingOrder(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Triggered skit " + name);

        if (other.gameObject.name == "Player")
            gc.StartCoroutine(gc.TriggerBattle(encounter, name));
    }
}
