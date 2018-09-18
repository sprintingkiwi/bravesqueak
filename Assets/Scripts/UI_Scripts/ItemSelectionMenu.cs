using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemSelectionMenu : Menu
{
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
                List<Skill> availableSkills = new List<Skill>();
                Jrpg.Log("Available Skills: ");
                foreach (Skill s in hero.unlockedSkills)
                    if (!hero.skills.Contains(s))
                    {
                        Jrpg.Log(s.name);
                        availableSkills.Add(s);
                    }

                // Setting the first selected element
                activeItem = availableSkills[index];
                maxItems = availableSkills.Count;

                break;

            case "Perk":
                // Creating a list of available perks
                List<Perk> availablePerks = new List<Perk>();
                Jrpg.Log("Available Perks: ");
                foreach (Perk p in gc.unlockedPerks)
                    if (!hero.perksPrefabs.Contains(p))
                    {
                        Jrpg.Log(p.name);
                        availablePerks.Add(p);
                    }

                // Setting the first selected element
                activeItem = availablePerks[index];
                maxItems = availablePerks.Count;
                break;
        }

        UpdateActiveItem();
    }

    public void UpdateActiveItem()
    {
        transform.Find("Active Item").GetComponent<SpriteRenderer>().sprite = activeItem.GetComponent<SpriteRenderer>().sprite;
    }

    public override void Update()
    {
        base.Update();

        if (inputManager.UpArrowDown())
            if (index > 0)
            {
                index -= 1;
                UpdateActiveItem();
            }

        if (inputManager.DownArrowDown())
            if (index < maxItems - 1)
            {
                index += 1;
                UpdateActiveItem();
            }
    }
}
