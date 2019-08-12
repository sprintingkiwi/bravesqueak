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

    void Start()
    {
        GameController.instance = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player" && !GameController.instance.inTransfer && active && !transfering)
        {
            GameController.instance.StartCoroutine(GameController.instance.ProcessTransfer(collision, this));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        GameController.instance.inTransfer = false;
        transfering = false;
    }    
}
