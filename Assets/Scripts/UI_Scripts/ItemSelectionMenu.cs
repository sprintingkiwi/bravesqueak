using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemSelectionMenu : Menu
{
    List<Item> availableItems = new List<Item>();
    public Item activeItem;
    int index = 0;
    int maxItems;

    public void SetupSelection(HeroBattler hero, int itemID, string pool)
    {
        base.Setup();

        switch (pool)
        {
            case "Skill":

                // Creating a list of available skills
                Jrpg.Log("Available Skills: ");
                foreach (Skill s in hero.unlockedSkills)
                    if (!hero.skills.Contains(s))
                    {
                        Jrpg.Log(s.name);
                        availableItems.Add(s);
                    }
                break;

            case "Perk":
                // Creating a list of available perks
                Jrpg.Log("Available Perks: ");
                foreach (Perk p in gc.unlockedPerks)
                    if (!hero.perksPrefabs.Contains(p))
                    {
                        Jrpg.Log(p.name);
                        availableItems.Add(p);
                    }
                break;
        }

        // Setting the first selected element
        activeItem = availableItems[index];
        maxItems = availableItems.Count;

        UpdateActiveItem();
    }

    public void UpdateActiveItem()
    {
        activeItem = availableItems[index];
        Jrpg.Log("Actual Item: " + activeItem.name);
        transform.Find("Active Item").GetComponent<SpriteRenderer>().sprite = activeItem.GetComponent<SpriteRenderer>().sprite;
    }

    public override void Update()
    {
        base.Update();

        if (inputManager.UpArrowDown())
            if (index > 0)
            {
                Jrpg.Log("Decrementing Item Index");
                index -= 1;
                UpdateActiveItem();
            }

        if (inputManager.DownArrowDown())
            if (index < maxItems - 1)
            {
                Jrpg.Log("Incrementing Item Index");
                index += 1;
                UpdateActiveItem();
            }
    }
}
