using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transfer : MonoBehaviour
{   
    public WorldMap destinationMap;
    public string destinationPlace;
    public Vector3 destinationPos;

    [Header("System")]
    public bool active = true;
    public bool transfering;
    GameController gc;    

    void Start()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player" && !gc.inTransfer && active && !transfering)
        {
            gc.StartCoroutine(gc.ProcessTransfer(collision, this));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        gc.inTransfer = false;
        transfering = false;
    }    
}
