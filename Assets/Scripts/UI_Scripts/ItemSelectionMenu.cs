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
    public Text nameText;
    public Text descriptionText;
    SpriteRenderer elementImg;
    public string pool;
    public int itemSlot;
    Transform items;
    public float menuItemsDistance;
    public List<Coroutine> movingCoroutines = new List<Coroutine>();
    public bool moving;
    float targetY;

    public void SetupSelection(HeroBattler hero, int itemSlot, string pool)
    {
        base.Setup();

        this.pool = pool;
        this.itemSlot = itemSlot;
        items = transform.Find("Items");
        switch (pool)
        {
            case "Skill":
                // Creating a list of available skills
                Jrpg.Log("Available Skills:");
                foreach (Skill s in GameController.Instance.unlockedSkills)
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
                foreach (Perk p in GameController.Instance.unlockedPerks)
                    if (p != null)
                    {
                        availableItems.Add(p);
                    }

                // Removing Perks held by other heroes
                //foreach (HeroBattler hb in GameController.Instance.heroes)
                //    foreach (Item av in availableItems.ToArray())
                //        if (hb.perks.Contains(av))
                //            availableItems.Remove(av);                

                // Logging names
                Jrpg.Log("Available Perks: ");
                foreach (Item av in availableItems)
                    Jrpg.Log(av.name);

                //Jrpg.Log("Selected Perk: " + hero.perks[itemSlot].name);
                //availableItems.Insert(0, hero.perks[itemSlot]);

                break;
        }

        // Instantiate stuff
        nameText = Instantiate(Resources.Load("Menu/ItemNameText") as GameObject, GameObject.Find("Canvas").transform).GetComponent<Text>();
        descriptionText = Instantiate(Resources.Load("Menu/ItemDescriptionText") as GameObject, GameObject.Find("Canvas").transform).GetComponent<Text>();
        for (int i = 0; i < availableItems.Count; i++)
        {
            Vector3 itemPos = new Vector3(items.position.x, items.position.y - menuItemsDistance * i, items.position.z);
            GameObject menuItem = Instantiate(Resources.Load("Menu/MenuItem") as GameObject, itemPos, Quaternion.identity, items);
            menuItem.GetComponent<SpriteRenderer>().sprite = availableItems[i].GetComponent<SpriteRenderer>().sprite;
        }

        UpdateActiveItem();
    }

    public void UpdateActiveItem()
    {
        // Setting the first selected element
        activeItem = availableItems[selectionIndex];
        Jrpg.Log("Actual Item: " + activeItem.name);
        maxItems = availableItems.Count;

        // Name text
        nameText.name = name + "_" + nameText.name; // Name of the generating menu before the text object under canvas to be able to easily destroy them after menu is destroyed
        // Description text
        descriptionText.name = name + "_" + descriptionText.name; // Name of the generating menu before the text object under canvas to be able to easily destroy them after menu is destroyed
        // Element image
        elementImg = transform.Find("Element").GetComponent<SpriteRenderer>();

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

        if (InputManager.Instance.UpArrowDown())
            //if (!moving)
                if (selectionIndex > 0)
                {
                    Jrpg.Log("Decrementing Item Index");
                    selectionIndex -= 1;
                    items.Translate(Vector3.down * menuItemsDistance);
                    //movingCoroutines.Add(StartCoroutine(MoveItems(-1)));
                    UpdateActiveItem();
                }

        if (InputManager.Instance.DownArrowDown())
            //if (!moving)
                if (selectionIndex < maxItems - 1)
                {
                    Jrpg.Log("Incrementing Item Index");
                    selectionIndex += 1;
                    items.Translate(Vector3.up * menuItemsDistance);
                    //movingCoroutines.Add(StartCoroutine(MoveItems(1)));
                    UpdateActiveItem();
                }

        //CONFIRM
        if (InputManager.Instance.ButtonADown())
        {
            switch (pool)
            {
                case "Skill":
                    Jrpg.Log(activeItem.name + " skill selected at slot " + itemSlot, "Visible");
                    GameController.Instance.unlockedHeroes[father.GetComponent<HeroMenu>().heroIndex].skills[itemSlot] = (Skill)activeItem;
                    break;

                //case "Perk":
                //    Jrpg.Log(activeItem.name + " perk selected at slot " + itemSlot, "Visible");
                //    GameController.Instance.unlockedHeroes[father.GetComponent<HeroMenu>().heroIndex].perks[itemSlot] = (Perk)activeItem;
                //    break;
            }
            father.gameObject.SetActive(true);
            father.Setup();
            MenuDestruction();
        }
    }

    void OnDestroy()
    {
        foreach (Coroutine c in movingCoroutines)
            StopCoroutine(c);
    }

    void OnDisable()
    {
        foreach (Coroutine c in movingCoroutines)
            StopCoroutine(c);
        items.position = new Vector3(items.position.x, targetY, items.position.z);
    }

    public IEnumerator MoveItems(int direction)
    {
        moving = true;

        targetY = items.position.y + (menuItemsDistance * direction);
        while (Mathf.Abs(items.transform.position.y - targetY) > 0.1f)
        {
            items.position = new Vector3(items.position.x, items.position.y + (0.05f * direction), items.position.z);
            yield return null;
        }
        items.position = new Vector3(items.position.x, targetY, items.position.z);
        movingCoroutines.Remove(movingCoroutines[0]);

        moving = false;
        yield return null;
    }
}
