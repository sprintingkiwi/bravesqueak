using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMenu : Menu
{
    HeroBattler activeHero;

	// Use this for initialization
	public void Setup()
    {

    }

    public virtual void ChangeItem(ItemSelector selector, string pool)
    {
        Jrpg.Log("Intantiating Item Selection Menu");
        ItemSelectionMenu selMenu = Instantiate(Resources.Load("Menu/ItemSelectionMenu") as GameObject).GetComponent<ItemSelectionMenu>();
        selMenu.Setup(activeHero, selector.transform.GetSiblingIndex() + 1, pool);
    }
}
