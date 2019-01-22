using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItemSelectionMenu : Menu
{
    public List<Item> availableItems = new List<Item>();
    public Item activeItem;
    int selectionIndex = 0;
    int maxItems;
    Text nameText;
    Text descriptionText;
    SpriteRenderer elementImg;
    public string pool;
    public int itemSlot;

    public void SetupSelection(HeroBattler hero, int itemSlot, string pool)
    {
        base.Setup();

        this.pool = pool;
        this.itemSlot = itemSlot;
        switch (pool)
        {
            case "Skill":
                // Creating a list of available skills
                Jrpg.Log("Available Skills:");
                foreach (Skill s in gc.unlockedSkills)
                    if (!hero.skills.Contains(s) && s != null)
                    {
                        Jrpg.Log(s.name);
                        availableItems.Add(s);
                    }
                Jrpg.Log("Selected Skill: " + hero.skills[itemSlot].name);
                availableItems.Insert(0, hero.skills[itemSlot]);
                break;

            case "Perk":
                // Creating a list of available perks                
                foreach (Perk p in gc.unlockedPerks)
                    if (p != null)
                    {
                        availableItems.Add(p);
                    }

                // Removing Perks held by other heroes
                foreach (HeroBattler hb in gc.heroes)
                    foreach (Item av in availableItems.ToArray())
                        if (hb.perks.Contains(av))
                            availableItems.Remove(av);                

                // Logging names
                Jrpg.Log("Available Perks: ");
                foreach (Item av in availableItems)
                    Jrpg.Log(av.name);

                Jrpg.Log("Selected Perk: " + hero.perks[itemSlot].name);
                availableItems.Insert(0, hero.perks[itemSlot]);

                break;
        }

        // Setting the first selected element
        activeItem = availableItems[selectionIndex];
        maxItems = availableItems.Count;

        // Name text 
        nameText = Instantiate(Resources.Load("Menu/ItemNameText") as GameObject, GameObject.Find("Canvas").transform).GetComponent<Text>();
        nameText.transform.position = Camera.main.WorldToScreenPoint(transform.Find("Name").position);
        // Name of the generating menu before the text object under canvas to be able to easily destroy them after menu is destroyed
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
        activeItem = availableItems[selectionIndex];
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
            if (selectionIndex > 0)
            {
                Jrpg.Log("Decrementing Item Index");
                selectionIndex -= 1;
                UpdateActiveItem();
            }

        if (inputManager.DownArrowDown())
            if (selectionIndex < maxItems - 1)
            {
                Jrpg.Log("Incrementing Item Index");
                selectionIndex += 1;
                UpdateActiveItem();
            }

        //CONFIRM
        if (inputManager.ButtonADown())
        {
            switch (pool)
            {
                case "Skill":
                    Jrpg.Log(activeItem.name + " skill selected at slot " + itemSlot, "Visible");
                    gc.unlockedHeroes[father.GetComponent<HeroMenu>().heroIndex].skills[itemSlot] = (Skill)activeItem;
                    break;

                case "Perk":
                    Jrpg.Log(activeItem.name + " perk selected at slot " + itemSlot, "Visible");
                    gc.unlockedHeroes[father.GetComponent<HeroMenu>().heroIndex].perks[itemSlot] = (Perk)activeItem;
                    break;
            }
            father.gameObject.SetActive(true);
            father.Setup();
            MenuDestruction();
        }
    }
}
