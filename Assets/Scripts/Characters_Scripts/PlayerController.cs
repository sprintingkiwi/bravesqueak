using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{
    InputManager inputManager;
    Joystick joy;
    public Vector3 lastCheckedPos4RandEncounters;
    public bool randomEncounters;

    float hor;
    float ver;
    int xMove;
    int yMove;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
        if (Jrpg.CheckPlatform() == "Mobile")
            joy = GameObject.Find("Joystick").GetComponent<Joystick>();
    }

    // Update is called once per frame
    public override void Update ()
    {
        base.Update();

        if (Jrpg.CheckPlatform() == "Mobile")
        {
            if (joy.direction != Vector2.zero && canMove)
                Move(joy.direction);
            else
                Stop();
        }
        else
        {
            hor = Input.GetAxis("Horizontal");
            if (hor > 0.1f)
                xMove = 1;
            else if (hor < -0.1f)
                xMove = -1;
            else
                xMove = 0;
            ver = Input.GetAxis("Vertical");
            if (ver > 0.1f)
                yMove = 1;
            else if (ver < -0.1f)
                yMove = -1;
            else
                yMove = 0;
            Vector2 axisInput = new Vector2(xMove, yMove);
            if (axisInput != Vector2.zero && canMove)
                Move(axisInput);
            else
                Stop();
        }

        // Party Menu
        if (Input.GetButtonDown("ButtonB"))
        {
            if (gc.situation != "Map")
                return;

            gc.currentMap.gameObject.SetActive(false);

            StartCoroutine(Jrpg.HeroesSelection(gc.unlockedHeroes, 3, PartySelectionCallback, gc.partyPrefabs.ToArray()));
        }

        // Random Encounters
        if (randomEncounters && gc.currentMap.randomEncounters.Length > 0)
            CheckRandomEncounter();

        //// Keydown
        //if (inputManager.DownArrow())
        //{
        //    Move(Vector2.down);
        //}
        //else if (inputManager.UpArrow())
        //{
        //    Move(Vector2.up);
        //}
        //if (inputManager.LeftArrow())
        //{
        //    Move(Vector2.left);
        //}
        //else if (inputManager.RightArrow())
        //{
        //    Move(Vector2.right);
        //}

        //// Keyup
        //if (inputManager.DownArrowUp())
        //{
        //    Stop();
        //}
        //else if (inputManager.UpArrowUp())
        //{
        //    Stop();
        //}
        //if (inputManager.LeftArrowUp())
        //{
        //    Stop();
        //}
        //else if (inputManager.RightArrowUp())
        //{
        //    Stop();
        //}
    }

    public void CheckRandomEncounter()
    {
        if ((transform.position - lastCheckedPos4RandEncounters).magnitude > 3)
        {
            if (Random.Range(0, 100) < gc.currentMap.randomEncountersRate)
            {
                Jrpg.Log("Triggering random battle");
                gc.StartCoroutine(gc.TriggerBattle(gc.currentMap.ChooseRandomEncounter(), "Random"));
            }
            lastCheckedPos4RandEncounters = transform.position;
        }
    }

    // Callback to update party with selection cache
    public void PartySelectionCallback()
    {
        gc.partyPrefabs.Clear();
        foreach(Battler b in gc.selectionCache)
        {
            gc.partyPrefabs.Add((HeroBattler)b);
        }

        // Check if at least 3 else add random
        int count = gc.partyPrefabs.Count;
        while (count < 3)
        {
            foreach (HeroBattler h in gc.unlockedHeroes)
                if (!gc.partyPrefabs.Contains(h))
                    gc.partyPrefabs.Add(h);
            count = gc.partyPrefabs.Count;
        }
    }
}
