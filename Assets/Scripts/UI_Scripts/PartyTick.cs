using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTick : MonoBehaviour
{
    public GameObject tick;

    [Header("System")]
    PartyMenu partyMenu;

    void Start()
    {
        partyMenu = transform.parent.parent.GetComponent<PartyMenu>();
    }

    void OnMouseDown()
    {
        if (tick == null)
        {
            tick = Instantiate(Resources.Load("Menu/Tick") as GameObject, transform);
        }
        else
        {
            Destroy(tick);
            tick = null;
        }
    }

}
