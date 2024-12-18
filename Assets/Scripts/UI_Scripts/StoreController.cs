﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StoreController : MonoBehaviour
{
    GameController ps;
    Transform panel;
    public GameObject storeSkillPrefab;
    Skill[] skillCollection;
    public HeroBattler selectedHero;
    public int heroIndex;
    public string focus;

	// Use this for initialization
	void Start ()
    {
        ps = GameController.Instance;
        
        panel = GameObject.Find("PANEL").transform;

        skillCollection = Resources.LoadAll<Skill>("Skills");
        foreach (Skill s in skillCollection)
            Debug.Log(s.name);

        focus = "Heroes";
        selectedHero = ps.partyPrefabs[0];

        SetupShoppableSkills(selectedHero);
	}
	
	// Update is called once per frame
	void Update ()
    {
        // UI focus
		if (InputManager.Instance.RightArrowDown())
        {
            focus = "Heroes";
        }
        else if (InputManager.Instance.LeftArrowDown())
        {
            focus = "Skills";
        }

        // Up and Down selection
        if (focus == "Heroes")
        {
            if (InputManager.Instance.DownArrowDown())
            {
                int index = ps.partyPrefabs.IndexOf(selectedHero) + 1;
                if (index < ps.partyPrefabs.Count)
                    selectedHero = ps.partyPrefabs[index];
                else
                    selectedHero = ps.partyPrefabs[0];

                Debug.Log("Selected " + selectedHero.name);
                SetupShoppableSkills(selectedHero);
            }
            else if (InputManager.Instance.UpArrowDown())
            {
                int index = ps.partyPrefabs.IndexOf(selectedHero) - 1;
                if (index >= 0)
                    selectedHero = ps.partyPrefabs[index];
                else
                    selectedHero = ps.partyPrefabs[ps.partyPrefabs.Count - 1];

                Debug.Log("Selected " + selectedHero.name);
                SetupShoppableSkills(selectedHero);
            }
        }
        else
        {
            if (InputManager.Instance.DownArrowDown())
            {

            }
            else if (InputManager.Instance.UpArrowDown())
            {

            }
        }
    }

    void SetupShoppableSkills(HeroBattler hero)
    {
        // Destroy previous skills
        foreach (Transform child in panel)
            Destroy(child.gameObject);

        // Create available skills for the selected hero
        Vector3 startPos = new Vector3(-3f, 2f, 0);
        foreach (Skill s in skillCollection.Where(s => s != null)) // && hero.availableJobs.Contains(s.shopRequirement.job)))
        {
            //Do stuff to initialize store skills
            //... TO DO

            startPos = new Vector3(startPos.x, startPos.y - 1, 0);
        }
    }
}
