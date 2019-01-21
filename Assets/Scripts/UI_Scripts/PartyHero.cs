using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyHero : MonoBehaviour {

    PartyMenu partyMenu;
    HeroMenu heroMenu;
    public int heroIndex;

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
        partyMenu.CreateHeroMenu(heroIndex);
    }
}
