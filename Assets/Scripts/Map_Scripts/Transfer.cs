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
        GameController.Instance = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player" && !GameController.Instance.inTransfer && active && !transfering)
        {
            GameController.Instance.StartCoroutine(GameController.Instance.ProcessTransfer(collision, this));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        GameController.Instance.inTransfer = false;
        transfering = false;
    }    
}
