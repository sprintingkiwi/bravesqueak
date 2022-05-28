using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transfer : MonoBehaviour
{   
    public string destinationMap;
    //public string destinationPlace;
    public Vector3 destinationPos;

    [Header("System")]
    public bool active = true;
    public bool transfering;

    void Start()
    {

    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Player" && !GameController.Instance.inTransfer && active && !transfering)
        {
            Debug.LogWarning("Map: " + destinationMap);
            GameController.Instance.StartCoroutine(GameController.Instance.ProcessTransfer(destinationMap, destinationPos));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        GameController.Instance.inTransfer = false;
        transfering = false;
    }    
}
