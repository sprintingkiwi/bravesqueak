using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItemSelectionMenu : Menu
{
    List<Item> availableItems = new List<Item>();
    public Item activeItem;
    int index = 0;
    int maxItems;
    Text nameText;
    Text descriptionText;
    SpriteRenderer elementImg;

    public void SetupSelection(HeroBattler hero, int itemID, string pool)
    {
        base.Setup();

        switch (pool)
        {
            case "Skill":

                // Creating a list of available skills
                Jrpg.Log("Available Skills: ");
                foreach (Skill s in hero.unlockedSkills)
                    if (!hero.skills.Contains(s) && s != null)
                    {
                        Jrpg.Log(s.name);
                        availableItems.Add(s);
                    }
                
                break;

            case "Perk":

                // Creating a list of available perks
                Jrpg.Log("Available Perks: ");
                foreach (Perk p in gc.unlockedPerks)
                    if (p != null)
                    {
                        availableItems.Add(p);
                    }

                // Removing Perks held by other heroes
                foreach (HeroBattler hb in gc.heroes)
                    foreach (Item av in availableItems.ToArray())
                        if (hb.perksPrefabs.Contains(av))
                            availableItems.Remove(av);

                // Logging names
                foreach (Item av in availableItems)
                    Jrpg.Log(av.name);

                break;
        }

        // Setting the first selected element
        activeItem = availableItems[index];
        maxItems = availableItems.Count;

        // Name text
        nameText = Instantiate(Resources.Load("Menu/ItemNameText") as GameObject, GameObject.Find("Canvas").transform).GetComponent<Text>();
        nameText.transform.position = Camera.main.WorldToScreenPoint(transform.Find("Name").position);
        nameText.name = name + "_" + nameText.name;

        // Description text
        descriptionText = Instantiate(Resources.Load("Menu/ItemDescriptionText") as GameObject, GameObject.Find("Canvas").transform).GetComponent<Text>();
        descriptionText.transform.position = Camera.main.WorldToScreenPoint(transform.Find("Description").position);
        descriptionText.name = name + "_" + descriptionText.name;

        // Element image
        elementImg = transform.Find("Element").GetComponent<SpriteRenderer>();

        UpdateActiveItem();
    }

    public void UpdateActiveItem()
    {
        activeItem = availableItems[index];
        Jrpg.Log("Actual Item: " + activeItem.name);

        // Item icon
        transform.Find("Active Item").GetComponent<SpriteRenderer>().sprite = activeItem.GetComponent<SpriteRenderer>().sprite;

        // Update item name and description text
        nameText.text = activeItem.name;
        descriptionText.text = activeItem.description;

        // Update Element image for skills
        if (activeItem.GetComponent<Skill>() != null)
        {
            elementImg.sprite = (Resources.Load("Icons/Elements/" + activeItem.GetComponent<Skill>().element.ToString()) as GameObject).GetComponent<SpriteRenderer>().sprite;
        }
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
