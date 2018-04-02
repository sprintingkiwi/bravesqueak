using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transfer : MonoBehaviour
{   
    public WorldMap destinationMap;
    public string destinationPlace;

    [Header("System")]
    public bool active = true;
    public bool transfering;
    GameController ps;    

    void Start()
    {
        ps = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player" && !ps.inTransfer && active && !transfering)
        {
            ps.StartCoroutine(ps.ProcessTransfer(collision, this));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        ps.inTransfer = false;
        transfering = false;
    }    
}
