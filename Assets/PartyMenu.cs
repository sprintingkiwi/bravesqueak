using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMenu : Menu {

    public Transform[] heroesImages;
    // Use this for initialization
    public override void Setup()
    {
        base.Setup();

        heroesImages = new Transform[8];

        for (int i = 0; i < 8; i++)
        {
            heroesImages[i] = transform.Find("HEROES").GetChild(i);
            heroesImages[i].GetComponent<SpriteRenderer>().sprite = gc.heroes[i].GetComponent<SpriteRenderer>().sprite;
        }
    }
}
