using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTick : MonoBehaviour
{
    public GameObject tick;

    [Header("System")]
    public PartyMenu partyMenu;
    public PartyHero partyHero;

    public void Setup()
    {
        partyMenu = transform.parent.parent.parent.GetComponent<PartyMenu>();
        partyHero = transform.parent.GetComponent<PartyHero>();

        GameController.instance = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    //void OnMouseDown()
    //{
    //    Select();
    //}

    

}
