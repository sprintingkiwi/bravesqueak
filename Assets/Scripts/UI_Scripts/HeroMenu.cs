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

        if (InputManager.instance.RightArrowDown())
        {
            if (heroIndex < GameController.instance.unlockedHeroes.Length - 1)
            {
                Jrpg.Log("Incrementing active hero index");
                heroIndex += 1;
                LoadHeroStuff();
            }
        }
        if (InputManager.instance.LeftArrowDown())
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
        Jrpg.Log("Loading " + GameController.instance.unlockedHeroes[heroIndex].name + " stuff");

        // Assign Active Hero sprite
        transform.Find("Active Hero").GetComponent<SpriteRenderer>().sprite = GameController.instance.unlockedHeroes[heroIndex].GetComponent<SpriteRenderer>().sprite;

        // Assign equipped items sprites
        foreach (Transform skillItem in transform.Find("Skills"))
        {
            if (GameController.instance.unlockedHeroes[heroIndex].skills[skillItem.GetSiblingIndex()] != null)
            {
                Jrpg.Log("Assigning sprite to skill " + skillItem.name);
                skillItem.GetComponent<SpriteRenderer>().sprite = GameController.instance.unlockedHeroes[heroIndex].skills[skillItem.GetSiblingIndex()].GetComponent<SpriteRenderer>().sprite;
            }            
        }
        foreach (Transform perkItem in transform.Find("Perks"))
        {
            if (GameController.instance.unlockedHeroes[heroIndex].perks[perkItem.GetSiblingIndex()] != null)
            {
                Jrpg.Log("Assigning sprite to perk " + perkItem.name);
                perkItem.GetComponent<SpriteRenderer>().sprite = GameController.instance.unlockedHeroes[heroIndex].perks[perkItem.GetSiblingIndex()].GetComponent<SpriteRenderer>().sprite;
            }            
        }
    }

    public void ShowItemSelection(ItemSelector selector, string pool)
    {
        if (GameController.instance.itemSelectionMenu != null)
            return;

        Jrpg.Log("Intantiating Item Selection Menu");
        GameController.instance.itemSelectionMenu = Instantiate(Resources.Load("Menu/ItemSelectionMenu") as GameObject, GameController.instance.mapCamera.transform).GetComponent<ItemSelectionMenu>();
        GameController.instance.itemSelectionMenu.father = this;
        subMenus.Add(GameController.instance.itemSelectionMenu);
        GameController.instance.itemSelectionMenu.SetupSelection(GameController.instance.unlockedHeroes[heroIndex], selector.transform.GetSiblingIndex(), pool);
        gameObject.SetActive(false);
    }
}
