﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMenu : Menu {

    public Transform[] heroesImages;
    public int ticks;
    // Use this for initialization
    public override void Setup()
    {
        base.Setup();

        heroesImages = new Transform[8];

        for (int i = 0; i < 8; i++)
        {
            heroesImages[i] = transform.Find("HEROES").GetChild(i);
            heroesImages[i].GetComponent<SpriteRenderer>().sprite = gc.heroes[i].GetComponent<SpriteRenderer>().sprite;
            heroesImages[i].GetComponent<Animator>().runtimeAnimatorController = gc.heroes[i].GetComponent<Animator>().runtimeAnimatorController;
        }
    }

    public void CreateHeroMenu()
    {
        gc.heroMenu = Instantiate(Resources.Load("Menu/HeroMenu") as GameObject, gc.mapCamera.transform).GetComponent<HeroMenu>();
        gc.heroMenu.father = this;
        subMenus.Add(gc.heroMenu);
        gc.heroMenu.Setup();
        gameObject.SetActive(false);
    }
}