using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMenu : Menu
{
    HeroBattler activeHero;
    ItemSelectionMenu selMenu;

	// Use this for initialization
	public override void Setup()
    {
        base.Setup();

        activeHero = gc.partyPrefabs[0];
    }

    public virtual void ChangeItem(ItemSelector selector, string pool)
    {
        if (selMenu != null)
            return;

        Jrpg.Log("Intantiating Item Selection Menu");
        selMenu = Instantiate(Resources.Load("Menu/ItemSelectionMenu") as GameObject, gc.mapCamera.transform).GetComponent<ItemSelectionMenu>();
        selMenu.SetupSelection(activeHero, selector.transform.GetSiblingIndex() + 1, pool);
    }
}
