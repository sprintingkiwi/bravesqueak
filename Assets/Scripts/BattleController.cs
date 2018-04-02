﻿using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
//using UnityEditor;

public class BattleController : MonoBehaviour
{
    public GameController gc;
    InputManager inputManager;

    [Header("Game Objects")]
    //public List<HeroBattler> party = new List<HeroBattler>();
    public Encounter encounter;
    public List<Encounter.Enemy> encounterEnemies = new List<Encounter.Enemy>();
    public List<Vector3> partyPositions = new List<Vector3>();
    public List<Vector3> enemiesPositions = new List<Vector3>();
    public GameObject battleback;
    public BattleMenu battleMenu;
    public Coroutine processChoiceCoroutine;
    public BattleCameraController mainCamera;

    [Header("Instantiated")]
    public List<Battler> party = new List<Battler>();
    public List<Battler> enemies = new List<Battler>();
    //public List<AIBattler> summons = new List<AIBattler>();

    [Header("Turn Management")]
    public List<BattleAction> actionsQueue = new List<BattleAction>();
    //public int actualTurn;
    //public bool actualActionEnded = true;
    public BattleAction actualAction;
    public List<Skill> ongoingSkills = new List<Skill>();
    public int executedOngoingSkills;
    public List<Coroutine> cameraCoroutines = new List<Coroutine>();
    public int turnNumber;
    public List<Func<BattleController, IEnumerator>> turnStartupBehaviours = new List<Func<BattleController, IEnumerator>>();
    public List<Func<BattleController, IEnumerator>> actionStartupBehaviours = new List<Func<BattleController, IEnumerator>>();

    [Header("System")]
    //public AudioSource audioSource;
    public GameObject battleMenuPrefab;
    public string outcome;

    // Use this for initialization
    public void Setup (Encounter encounter)
    {
        this.encounter = encounter;

        mainCamera = GameObject.Find("Battle Camera").GetComponent<BattleCameraController>();
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();

        // Retrieving data from Game Controller
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        gc.canSave = false;
        //party = ps.party;
        //enemies = persistentStuff.enemies;
        foreach (Encounter.Enemy ee in encounter.enemies)
            encounterEnemies.Add(ee);

        // Battleback
        // If there is no encounter-specific battleback then load the map default bb
        if (encounter.battleback != null)
            battleback = encounter.battleback;
        else
            battleback = gc.currentMap.defaultBattleback;
        
        // SETUP
        SetupBattlers();
        SetupBattleback(battleback);

        // Populate turns
        turnNumber = 0;
        StartCoroutine(mainCamera.Move(encounter.cameraAdjust.delta, encounter.cameraAdjust.speed));
        StartCoroutine(ManageTurns());
        Jrpg.Fade(GameObject.Find("Intro"), 0, 1);
    }

    // Update is called once per frame
    void Update ()
    {
        DebugCheats();
                

    }

    static int SortByPosY(Encounter.Enemy b1, Encounter.Enemy b2)
    {
        return b1.place.transform.position.y.CompareTo(b2.place.transform.position.y);
    }

    void SetupBattlers()
    {
        // Instantiate battlers
        // Heroes
        for (int i = 0; i < gc.partyPrefabs.Count; i++)
        {
            HeroBattler h = Instantiate(gc.partyPrefabs[i], partyPositions[i], Quaternion.identity, gc.battleStuff.transform) as HeroBattler;
            h.name = h.job.ToString();
            //h.GetComponent<SpriteRenderer>().sortingOrder = 4 - i;

            if (encounter.partyPosAdjust != null)
                h.transform.Translate(encounter.partyPosAdjust.position);

            party.Add(h);
            h.Setup();
        }
        // Enemies
        encounterEnemies.Sort(SortByPosY);
        for (int i = 0; i < encounterEnemies.Count; i++)
        {
            // This was needed to add a custom offset determined by the enemy prefab's transform
            // ...but now I'm setting encounters enemies positions manually in the encounters
            // prefab, so it's not necessary anymore.
            //Vector3 enPos = new Vector3(enemiesPositions[i].x + enemies[i].transform.position.x, enemiesPositions[i].y + enemies[i].transform.position.y, enemies[i].transform.position.z);

            EnemyBattler e = Instantiate(encounterEnemies[i].recipe, encounterEnemies[i].place.transform.position, Quaternion.identity, gc.battleStuff.transform) as EnemyBattler;
            e.prefabName = e.name.Remove(e.name.Length - 7);
            e.name = e.species.ToString() + " " + i.ToString();
            
            // Rename enemy
            if (e.characterName != "")
                e.name = e.characterName + " (" + i.ToString() + ")";

            // Determine sorting layer
            // This too is needed only if enemies are automatically positioned, and I'm still not sure about which would be the best choice
            e.GetComponent<SpriteRenderer>().sortingOrder = 4 - i;

            enemies.Add(e);
            e.Setup();
        }
    }

    void SetupBattleback(GameObject battleback)
    {
        // This is for the direct load from Resources asset folder
        Debug.Log("Loading battleback: " + battleback);

        GameObject bb = Instantiate(battleback, gc.battleStuff.transform);
        bb.name = "BATTLEBACK";

        // Load encounter-specific battleback layers
        foreach (Transform b in encounter.transform.Find("CustomBattlebackLayers"))
            Instantiate(b, bb.transform);
    }

    Coroutine SetupBattleMenu(HeroBattler playerBattler)
    {
        battleMenu = Instantiate(battleMenuPrefab, mainCamera.gameObject.transform).GetComponent<BattleMenu>();
        battleMenu.name = "Battle Menu";
        //battleMenu.instParty = instParty;
        //battleMenu.instEnemies = instEnemies;
        battleMenu.playerBattler = playerBattler;
        return battleMenu.Setup();
    }

    static int SortBySpeed(BattleAction b1, BattleAction b2)
    {
        return (Jrpg.Roll(b1.user.speed, modifier: b1.skill.speed).CompareTo(Jrpg.Roll(b2.user.speed, modifier: b2.skill.speed)));
    }

    public void SetupActionsQueue()
    {
        actionsQueue.Sort(SortBySpeed);
        actionsQueue.Reverse();
    }

    IEnumerator ManageTurns()
    {
        while (EvaluateBattleOutcome() == "Continue")
        {
            // Turn Setup
            actionsQueue.Clear();
            turnNumber += 1;

            // AI Actions
            foreach (Battler b in enemies.Cast<AIBattler>().Concat(party.Where(s => s != null && s.GetComponent<Summon>() != null).Cast<AIBattler>()))
            {                
                AIBattler e = b.GetComponent<AIBattler>();
                e.ApplyStrategy();
            }
            yield return new WaitForSeconds(0.1f);

            // Player Actions
            foreach (Battler b in party.Where(s => s != null && s.GetComponent<HeroBattler>() != null))
            {
                yield return SetupBattleMenu(b.GetComponent<HeroBattler>());
                yield return new WaitForSeconds(0.1f);
            }

            // Order of execution
            SetupActionsQueue();

            // Turn Startup Custom Functions
            foreach (Func<BattleController, IEnumerator> tsb in turnStartupBehaviours)
                yield return StartCoroutine(tsb(this));

            for (int i = 0; i < actionsQueue.Count; i++)
            {
                // Update public actual action ref
                actualAction = actionsQueue[i];

                // Action Startup Custom Functions
                foreach (Func<BattleController, IEnumerator> asb in actionStartupBehaviours)
                    yield return StartCoroutine(asb(this));

                // Check if the user is still alive
                if (actionsQueue[i].user != null)
                {
                    // Log
                    Debug.Log(actionsQueue[i].user.name + " is still alive and can act");
                    Jrpg.Log(actionsQueue[i].user.name + " action");
                    yield return new WaitForSeconds(0.1f);

                    // Camera movement   
                    foreach (Coroutine c in cameraCoroutines)
                    {
                        if (c != null)
                            StopCoroutine(c);
                    }
                    cameraCoroutines.Add(StartCoroutine(mainCamera.DefaultFollow(actionsQueue[i].user)));

                    // Apply status effects
                    actionsQueue[i].user.ProcessStatusEffects();

                    // Process ongoing skills
                    Debug.Log("Processing ongoing skills effects");
                    foreach (Skill s in ongoingSkills.ToArray())
                    {
                        // Update targets for Area Ongoing skills (in case a new battler join the battle in the middle of it, following a summon for example)
                        if (s.scope == Skill.Scope.Area)
                        {
                            s.targets.Clear();
                            foreach (Battler b in enemies.Cast<Battler>().Concat(party.Cast<Battler>()))
                                if (b.faction == s.targetedArea)
                                    s.targets.Add(b);
                        }

                        // Process skills Ongoing-Effects and Post-Effects
                        yield return new WaitForSeconds(0.1f);
                        s.effectStillActive = true;
                        if (s.duration >= 1)
                        {
                            yield return StartCoroutine(s.OngoingFlow());
                            //while (!actualActionEnded)
                            //yield return null;
                            //actualActionEnded = false;
                            s.duration -= 1;
                        }
                        else
                        {
                            yield return StartCoroutine(s.PostFlow());
                            ongoingSkills.Remove(s);
                            Destroy(s.gameObject);
                        }
                    }

                    // Update battlers stats adding delta stats
                    Debug.Log("Updating battlers STATS");
                    actionsQueue[i].user.UpdateStats();
                    // Warmups and Cooldowns
                    //actionQueue[i].user.ProcessWarmupsAndCooldowns();

                    foreach (Battler t in actionsQueue[i].targets.ToArray())
                    {
                        if (t == null)
                        {
                            Debug.Log("Detected deprecated target for " + actionsQueue[i].user.name + " " + actionsQueue[i].skill.name);
                            actionsQueue[i].targets = Strategy.ChooseRandomTarget(actionsQueue[i].user, actionsQueue[i].skill, enemies.ToArray(), party.ToArray());
                        }
                    }
                    // Execute action
                    yield return actionsQueue[i].user.UseSkill(actionsQueue[i].skill, actionsQueue[i].targets);
                }

                outcome = EvaluateBattleOutcome();
                if (outcome != "Continue")
                    break;
            }
        }

        // BATTLE END:
        gc.StartCoroutine(gc.EndBattle(outcome, encounter));

        //gc.canSave = true;
        //gc.lastBattleOutcome = EvaluateBattleOutcome();

        // Experience gain
        //Debug.LogError("Write Exp");
        //for (int i = 0; i < ps.party.Count; i++)
        //{
        //    Debug.Log("Writing " + instParty[i].name + " exp in " + ps.party[i].name);
        //    foreach (KeyValuePair<string, int> exp in ps.party[i].experience)
        //        ps.party[i].experience[exp.Key] += instParty[i].experience[exp.Key];
        //}

        //else
        //gc.defeatedNormalEnemies.Add(gc.currentEnemy);

        // Load world scene
        //if (EvaluateBattleOutcome() == "Win")
        //{
        //    StartCoroutine(Jrpg.LoadScene("World", new Coroutine[] { StartCoroutine(gc.SetVolume(0)) }));
        //}
        //else
        //{
        //    StartCoroutine(Jrpg.LoadScene("MainMenu"));
        //}
    }

    public string EvaluateBattleOutcome()
    {
        if (enemies.Count > 0)
            return "Continue";
        else if (party.Count < 1)
            return "Lose";
        else
            return "Win";
    }

    void DebugCheats()
    {
        if (Input.GetKeyDown(KeyCode.W))
            enemies.Clear();
    }
}
