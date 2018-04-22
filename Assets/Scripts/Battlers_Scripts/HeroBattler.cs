﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class HeroBattler : Battler
{
    public enum Job { None, Swordmaster, Barbarian, Dragoon, Paladin, Wizard, Countermage, Warlock, Cleric, Ranger, Gunslinger, Ninja };

    [Header("Hero battler specific")]
    public Job job;
    public Job[] availableJobs;
    public List<Skill> unlockedSkills = new List<Skill>();
    public Skill[] skills = new Skill[5];    

    // Use this for initialization
    public override void Setup()
    {
        base.Setup();

        faction = Faction.Heroes;

        //sideMod = -1;

        //originalPos = transform.position;

        // Load anim controller based on equip preset
        //Debug.Log("Attaching animator controller to " + name);
        //animator.runtimeAnimatorController = Resources.Load("Battlers/Heroes/" + job.ToString() + "/Controllers/" + equipmentPreset + ".controller") as RuntimeAnimatorController;

        // Setting HP
        //maxHP.value = (constitution.value * level) / 2;
        // Setting SP
        //maxSP.value = (magicAttack.value * level) / 3;
        //skillPoints = maxSP.value;

        // Calculate Base Attack Bonus
        //baseAttackBonus.value = baseAttackBonusTable[level];
        // Calculate Base Shield Bonus        

        //col = gameObject.GetComponent<PolygonCollider2D>();
        //UpdateAnimator();

        //sprForCollider = gameObject.GetComponent<SpriteRenderer>();
        //sprForCollider.sprite = transform.Find("JOBS").Find(job.ToString()).GetComponent<SpriteRenderer>().sprite;
        //SetupCollider();

        //UpdateSortingOrder();

        // Another update for delta stats because of perks effects and maybe other hero-specific stuff
        UpdateStats();

        // PERKS        
        SetupPerks();
    }

    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
    }    

    // Choose the right animator based on the active job
    public void UpdateAnimator()
    {
        foreach (Transform j in transform.Find("JOBS"))
        {
            if (j.name == job.ToString())
            {
                j.gameObject.SetActive(true);
                anim = j.GetComponent<Animator>();
                spr = j.GetComponent<SpriteRenderer>();
            }
            else
                j.gameObject.SetActive(false);
        }

        // Refresh Polygon Collider copying path(s) points from the template collider (under JOBS child) to the collider attached to the parent HeroBattler GameObject
        Destroy(col);
        col = gameObject.AddComponent<PolygonCollider2D>();
        col.isTrigger = true;
        PolygonCollider2D templateCol = transform.Find("JOBS").Find(job.ToString()).GetComponent<PolygonCollider2D>();
        
        // this works copying one path, despite it doesn't copy all the paths if more than one
        col.points = templateCol.points;
        
        // this raises a error but I don't know why...
        //for (int i = 0; i < templateCol.pathCount; i++)
            //col.SetPath(i, templateCol.GetPath(i));
    }
}
