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
    public HeroBattler[] availables;
    public int selectables;
    public HeroBattler[] alreadySelected;
    public GameObject heroUI;
    public bool forceSelectMaximum;

    Coroutine heroUICoroutine;
    Transform heroes;

    // Use this for initialization
    public override void Setup()
    {
        base.Setup();
    }

    public void SelectionSetup(HeroBattler[] availables, int selectables, HeroBattler[] alreadySelected=null)
    {
        this.availables = availables;
        this.selectables = selectables;
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
            if (alreadySelected != null)
                if (alreadySelected.Contains(availables[i]))
                    Select(heroesImages[i].GetComponent<PartyHero>());
        }

        highlighter = transform.Find("Highlight").gameObject;
    }

    public override void Update()
    {
        // Wait to destroy hero description if it's being showed
        if (currentHeroDesc != null)
            if (Input.anyKeyDown)
            {
                Destroy(currentHeroDesc);
                return;
            }

        // Manage selection if not inside UI coroutine
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

    public void ShowHeroDescription(HeroBattler hero)
    {
        Jrpg.Log("Displaying " + name + " description");
        currentHeroDesc = Instantiate(hero.UIPrefab, transform);
        currentHeroDesc.transform.Find("Background").GetComponent<Image>().sprite = Sprite.Create(hero.DescriptionBG, new Rect(0, 0, hero.DescriptionBG.width, hero.DescriptionBG.height), new Vector2(0, 0));
        currentHeroDesc.transform.Find("Battler").GetComponent<Image>().sprite = hero.GetComponent<SpriteRenderer>().sprite;
        currentHeroDesc.transform.Find("Text").GetComponent<Text>().text = hero.description;
        foreach (Image skillImg in currentHeroDesc.transform.Find("Skills").GetComponentsInChildren<Image>())
        {
            Skill skill = hero.skills[skillImg.transform.GetSiblingIndex()];
            skillImg.sprite = skill.GetComponent<SpriteRenderer>().sprite;
            skillImg.GetComponentInChildren<Text>().text = skill.description.ToUpper();
        }
        currentHeroDesc.transform.Find("Element").Find("Image").GetComponent<Image>().sprite = (Resources.Load("Icons/Elements/" + hero.elementAffinity.ToString()) as GameObject).GetComponent<SpriteRenderer>().sprite;
        currentHeroDesc.transform.Find("Battler").position += hero.correctHeroPosition;
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
        highlighter.SetActive(true);

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

        // Write Select/Deselect
        if (GameController.instance.selectionCache.Contains(availables[index]))
        {
            hu.transform.Find("Select").GetComponentInChildren<Text>().text = "Remove";
        }

        // Wait until menu button selection
        int UIindex = 0;
        Image selectedImg = hu.transform.GetChild(0).GetComponent<Image>();
        yield return null;
        while (!InputManager.instance.ButtonADown())
        {
            // Highlight current button
            foreach(Transform t in hu.transform)
            {
                t.GetComponent<Image>().color = Color.grey;
            }
            selectedImg = hu.transform.GetChild(UIindex).GetComponent<Image>();
            selectedImg.color = Color.yellow;
            if (selectedImg.name == "Select" && ticks >= selectables && !GameController.instance.selectionCache.Contains(availables[index]))
            {
                selectedImg.color = Color.red;
            }

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

        if (selectedImg.name == "Select")
            Select(heroesImages[index].GetComponent<PartyHero>());
        else
            ShowHeroDescription(availables[index]);

        heroUICoroutine = null;
        Destroy(hu);
        yield return null;
    }

    public void Select(PartyHero partyHero)
    {

        if (partyHero.tick == null)
        {
            if (ticks >= selectables)
                return;

            partyHero.tick = Instantiate(Resources.Load("Menu/Tick") as GameObject, partyHero.transform.Find("Square").transform);
            ticks += 1;

            //// Add Hero to Party
            //partyMenu.GameController.instance.partyPrefabs[partyMenu.ticks - 1] = partyMenu.GameController.instance.unlockedHeroes[transform.parent.GetSiblingIndex()];

            // Add hero to cache list of selected heroes
            foreach (HeroBattler h in GameController.instance.heroes)
            {
                if (h.name == availables[partyHero.heroIndex].name)
                    GameController.instance.selectionCache.Add(h);
            }

        }
        else
        {
            Destroy(partyHero.tick);
            partyHero.tick = null;
            ticks -= 1;

            // Remove hero from cache list of selected heroes
            foreach (HeroBattler h in GameController.instance.heroes)
            {
                if (h.name == availables[partyHero.heroIndex].name)
                    GameController.instance.selectionCache.Remove(h);
            }
        }
    }

    public override void MenuDestruction()
    {
        if (forceSelectMaximum) // Force the user to select the maximum number of selectable heroes
            if (GameController.instance.selectionCache.Count < selectables)
                return;

        base.MenuDestruction();
    }

}
