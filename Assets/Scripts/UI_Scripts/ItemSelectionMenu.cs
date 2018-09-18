using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemSelectionMenu : Menu
{
    Item activeItem;
    int index = 0;
    int maxItems;
    InputManager inputManager;

    public void Setup(HeroBattler hero, int itemID, string pool)
    {
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();

        switch (pool)
        {
            case "Skill":

                // Creating a list of available skills
                List<Skill> availableSkills = new List<Skill>();
                foreach (Skill s in hero.unlockedSkills)
                    if (hero.skills.Contains(s))
                        availableSkills.Add(s);

                // Setting the first selected element
                activeItem = availableSkills[index];
                maxItems = availableSkills.Count;

                break;

            case "Perk":
                break;
        }

        UpdateActiveItem();
    }

    public void UpdateActiveItem()
    {
        transform.Find("Active Item").GetComponent<SpriteRenderer>().sprite = activeItem.GetComponent<SpriteRenderer>().sprite;
    }

    void Update()
    {
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
