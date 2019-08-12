using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PartyMenu : Menu {

    [Header("Party Menu")]
    public GameObject partyHeroPrefab;
    public Transform[] heroesImages;
    public int ticks;
    public GameObject currentHeroDesc;
    public GameObject highlighter;
    public int index;
    public Battler[] availables;
    public Battler[] alreadySelected;
    public GameObject heroUI;
    Coroutine heroUICoroutine;

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

        heroesImages = new Transform[availables.Length];

        heroes = transform.Find("HEROES");

        // Get list of position hooks
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < availables.Length; i++)
        {
            positions.Add(heroes.GetChild(i).position);
        }

        // Destroy hooks
        foreach (Transform t in heroes)
            Destroy(t.gameObject);

        // Populates hero objects
        for (int i = 0; i < availables.Length; i++)
        {            
            heroesImages[i] = Instantiate(partyHeroPrefab, positions[i], Quaternion.identity, heroes).transform;
            heroesImages[i].gameObject.name = availables[i].name + "_Menu";
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
        if (heroUICoroutine == null)
            SelectionManagement();
    }

    public void CreateHeroMenu(int index)
    {
        GameController.instance.heroMenu = Instantiate(Resources.Load("Menu/HeroMenu") as GameObject, GameController.instance.mapCamera.transform).GetComponent<HeroMenu>();
        GameController.instance.heroMenu.father = this;
        subMenus.Add(GameController.instance.heroMenu);
        GameController.instance.heroMenu.heroIndex = index;
        GameController.instance.heroMenu.Setup();
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
        if (InputManager.instance.RightArrowDown())
        {
            if (index < heroes.childCount - 1)
                index += 1;
            else
                index = 0;
        }
        if (InputManager.instance.LeftArrowDown())
        {
            if (index > 0)
                index -= 1;
            else
                index = heroes.childCount - 1;
        }

        if (InputManager.instance.DownArrowDown())
        {
            if (index < heroes.childCount - 4)
                index += 4;
            else
                index = 3;
        }
        if (InputManager.instance.UpArrowDown())
        {
            if (index > 3)
                index -= 4;
            else
                index = heroes.childCount - 4;
        }

        // Positioning highlighter
        highlighter.transform.position = transform.Find("HEROES").GetChild(index).position;

        // Cancel
        if (InputManager.instance.ButtonBDown())
        {
            if (currentHeroDesc == null)
                MenuDestruction();
            else
            {
                Destroy(currentHeroDesc);
            }
        }

        // Confirm
        if (InputManager.instance.ButtonADown())
        {
            heroUICoroutine = StartCoroutine(ManageHeroUI());
        }
    }

    public IEnumerator ManageHeroUI()
    {
        GameObject hu = Instantiate(heroUI, GameObject.Find("Canvas").transform);
        hu.transform.position = Camera.main.WorldToScreenPoint(highlighter.transform.position);

        // Wait until menu button selection
        int UIindex = 0;
        yield return null;
        while (!InputManager.instance.ButtonADown())
        {
            // Highlight current button
            foreach(Transform t in hu.transform)
            {
                t.GetComponent<Image>().color = Color.grey;
            }
            Image selectedImg = hu.transform.GetChild(UIindex).GetComponent<Image>();
            selectedImg.color = Color.yellow;

            // UI selection management
            if (InputManager.instance.DownArrowDown())
                if (UIindex < hu.transform.childCount - 1)
                    UIindex += 1;
            if (InputManager.instance.UpArrowDown())
                if (UIindex > 0)
                    UIindex -= 1;
            if (InputManager.instance.ButtonBDown())
            {
                heroUICoroutine = null;
                Destroy(hu);
                yield break;
            }

            yield return null;
        }

        ConfirmHeroSelection();
        Destroy(hu);

        yield return null;
    } 

    public void ConfirmHeroSelection()
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
