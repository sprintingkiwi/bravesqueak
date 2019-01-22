using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMenu : Menu
{
    public int heroIndex;

	// Use this for initialization
	public override void Setup()
    {
        base.Setup();

        LoadHeroStuff();
    }

    public override void Update()
    {
        base.Update();

        if (inputManager.RightArrowDown())
        {
            if (heroIndex < gc.unlockedHeroes.Length - 1)
            {
                Jrpg.Log("Incrementing active hero index");
                heroIndex += 1;
                LoadHeroStuff();
            }
        }
        if (inputManager.LeftArrowDown())
        {
            if (heroIndex > 0)
            {
                Jrpg.Log("Decrementing active hero index");
                heroIndex -= 1;
                LoadHeroStuff();
            }
        }
    }

    public void LoadHeroStuff()
    {
        Jrpg.Log("Loading " + gc.unlockedHeroes[heroIndex].name + " stuff");

        // Assign Active Hero sprite
        transform.Find("Active Hero").GetComponent<SpriteRenderer>().sprite = gc.unlockedHeroes[heroIndex].GetComponent<SpriteRenderer>().sprite;

        // Assign equipped items sprites
        foreach (Transform skillItem in transform.Find("Skills"))
        {
            if (gc.unlockedHeroes[heroIndex].skills[skillItem.GetSiblingIndex()] != null)
            {
                Jrpg.Log("Assigning sprite to skill " + skillItem.name);
                skillItem.GetComponent<SpriteRenderer>().sprite = gc.unlockedHeroes[heroIndex].skills[skillItem.GetSiblingIndex()].GetComponent<SpriteRenderer>().sprite;
            }            
        }
        foreach (Transform perkItem in transform.Find("Perks"))
        {
            if (gc.unlockedHeroes[heroIndex].perks[perkItem.GetSiblingIndex()] != null)
            {
                Jrpg.Log("Assigning sprite to perk " + perkItem.name);
                perkItem.GetComponent<SpriteRenderer>().sprite = gc.unlockedHeroes[heroIndex].perks[perkItem.GetSiblingIndex()].GetComponent<SpriteRenderer>().sprite;
            }            
        }
    }

    public void ShowItemSelection(ItemSelector selector, string pool)
    {
        if (gc.itemSelectionMenu != null)
            return;

        Jrpg.Log("Intantiating Item Selection Menu");
        gc.itemSelectionMenu = Instantiate(Resources.Load("Menu/ItemSelectionMenu") as GameObject, gc.mapCamera.transform).GetComponent<ItemSelectionMenu>();
        gc.itemSelectionMenu.father = this;
        subMenus.Add(gc.itemSelectionMenu);
        gc.itemSelectionMenu.SetupSelection(gc.unlockedHeroes[heroIndex], selector.transform.GetSiblingIndex(), pool);
        gameObject.SetActive(false);
    }
}
