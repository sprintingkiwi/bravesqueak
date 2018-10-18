using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMenu : Menu
{
    int index;
    ItemSelectionMenu selMenu;

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
            if (index < gc.partyPrefabs.Count - 1)
            {
                Jrpg.Log("Incrementing active hero index");
                index += 1;
                LoadHeroStuff();
            }
        }
        if (inputManager.LeftArrowDown())
        {
            if (index > 0)
            {
                Jrpg.Log("Decrementing active hero index");
                index -= 1;
                LoadHeroStuff();
            }
        }
    }

    public void LoadHeroStuff()
    {
        // Assign Active Hero sprite
        transform.Find("Active Hero").GetComponent<SpriteRenderer>().sprite = gc.partyPrefabs[index].GetComponent<SpriteRenderer>().sprite;

        // Assign equipped items sprites
        foreach (Transform skillItem in transform.Find("Skills"))
        {
            if (gc.partyPrefabs[index].skills[skillItem.GetSiblingIndex()] != null)
            {
                Jrpg.Log("Assigning sprite to skill " + skillItem.name);
                skillItem.GetComponent<SpriteRenderer>().sprite = gc.partyPrefabs[index].skills[skillItem.GetSiblingIndex()].GetComponent<SpriteRenderer>().sprite;
            }            
        }
        foreach (Transform perkItem in transform.Find("Perks"))
        {
            if (gc.partyPrefabs[index].perksPrefabs[perkItem.GetSiblingIndex()] != null)
            {
                Jrpg.Log("Assigning sprite to perk " + perkItem.name);
                perkItem.GetComponent<SpriteRenderer>().sprite = gc.partyPrefabs[index].perksPrefabs[perkItem.GetSiblingIndex()].GetComponent<SpriteRenderer>().sprite;
            }            
        }
    }

    public void ChangeItem(ItemSelector selector, string pool)
    {
        if (selMenu != null)
            return;

        Jrpg.Log("Intantiating Item Selection Menu");
        selMenu = Instantiate(Resources.Load("Menu/ItemSelectionMenu") as GameObject, gc.mapCamera.transform).GetComponent<ItemSelectionMenu>();
        selMenu.SetupSelection(gc.partyPrefabs[index], selector.transform.GetSiblingIndex() + 1, pool);
    }
}
