using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyHero : MonoBehaviour {    

    [Header("System")]
    public int heroIndex;
    public GameObject tick;

    PartyMenu partyMenu;
    HeroMenu heroMenu;

    void Start()
    {
        partyMenu = transform.parent.parent.GetComponent<PartyMenu>();
    }

    //void OnMouseEnter()
    //{
    //    gameObject.GetComponent<Animator>().SetTrigger("attack");
    //}

    //void OnMouseDown()
    //{
    //    //// Used to Create Hero Menu
    //    //partyMenu.CreateHeroMenu(heroIndex);

    //    // Now just displaying the Hero's description Image
    //    partyMenu.ShowHeroDescription(partyMenu.availables[heroIndex].battlerDescription);
    //}
}
