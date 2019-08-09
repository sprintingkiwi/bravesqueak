using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PartyMenu : Menu {

    public Transform[] heroesImages;
    public int ticks;
    public GameObject currentHeroDesc;
    public GameObject highlighter;
    public int index;
    public Battler[] availables;
    public Battler[] alreadySelected;

    Transform heroes;

    // Use this for initialization
    public override void Setup()
    {
        base.Setup();
    }

    public void SelectionSetup(Battler[] availables, Battler[] alreadySelected=null)
    {
        this.availables = availables;
        this.alreadySelected = alreadySelected;

        heroesImages = new Transform[8];

        heroes = transform.Find("HEROES");

        for (int i = 0; i < availables.Length; i++)
        {
            transform.Find("HEROES").GetChild(i).gameObject.SetActive(true);
            heroesImages[i] = heroes.GetChild(i);
            heroesImages[i].GetComponent<SpriteRenderer>().sprite = availables[i].GetComponent<SpriteRenderer>().sprite;
            heroesImages[i].GetComponent<Animator>().runtimeAnimatorController = availables[i].GetComponent<Animator>().runtimeAnimatorController;
            PartyHero ph = heroesImages[i].GetComponent<PartyHero>();
            ph.heroIndex = i;
        }

        // Select already selected heroes
        for (int i = 0; i < availables.Length; i++)
        {
            PartyTick pt = heroesImages[i].GetComponentInChildren<PartyTick>();
            pt.Setup();

            if (alreadySelected != null)
                if (alreadySelected.Contains(availables[i]))
                    pt.Select();
        }

        highlighter = transform.Find("Highlight").gameObject;
    }

    public override void Update()
    {
        SelectionManagement();

        if (inputManager.ButtonBDown())
        {
            if (currentHeroDesc == null)
                MenuDestruction();
            else
            {
                Destroy(currentHeroDesc);
            }
        }        

        if (inputManager.ButtonADown())
        {
            if (currentHeroDesc == null)
            {
                ShowHeroDescription(heroes.GetChild(index).GetComponent<PartyHero>().heroDescription);
            }
            else
            {
                heroes.GetChild(index).GetComponentInChildren<PartyTick>().Select();
                Destroy(currentHeroDesc);
            }
        }
    }

    public void CreateHeroMenu(int index)
    {
        gc.heroMenu = Instantiate(Resources.Load("Menu/HeroMenu") as GameObject, gc.mapCamera.transform).GetComponent<HeroMenu>();
        gc.heroMenu.father = this;
        subMenus.Add(gc.heroMenu);
        gc.heroMenu.heroIndex = index;
        gc.heroMenu.Setup();
        gameObject.SetActive(false);
    }

    public void ShowHeroDescription(GameObject heroDescription)
    {
        Jrpg.Log("Displaying " + name + " description");
        currentHeroDesc = Instantiate(heroDescription, transform);
    }

    public void SelectionManagement()
    {
        // Move index based on arrows
        if (inputManager.RightArrowDown())
        {
            if (index < heroes.childCount - 1)
                index += 1;
            else
                index = 0;
        }
        if (inputManager.LeftArrowDown())
        {
            if (index > 0)
                index -= 1;
            else
                index = heroes.childCount - 1;
        }

        // Positioning highlighter
        highlighter.transform.position = transform.Find("HEROES").GetChild(index).position;                
    }

}
