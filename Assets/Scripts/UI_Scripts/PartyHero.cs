using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyHero : MonoBehaviour {

    PartyMenu partyMenu;
    HeroMenu heroMenu;

    void Start()
    {
        partyMenu = transform.parent.parent.GetComponent<PartyMenu>();
    }

    void OnMouseEnter()
    {
        gameObject.GetComponent<Animator>().SetTrigger("attack");
    }

    void OnMouseDown()
    {
        heroMenu = Instantiate(Resources.Load("Menu/HeroMenu") as GameObject, partyMenu.gc.mapCamera.transform).GetComponent<HeroMenu>();
        heroMenu.Setup();
    }
}
