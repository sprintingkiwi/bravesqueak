using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character
{
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

        InputManager.instance = GameObject.Find("Input Manager").GetComponent<InputManager>();
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
            if (GameController.instance.situation != "Map")
                return;

            GameController.instance.currentMap.gameObject.SetActive(false);

            StartCoroutine(Jrpg.HeroesSelection(GameController.instance.unlockedHeroes, 3, PartySelectionCallback, GameController.instance.partyPrefabs.ToArray()));
        }

        // Random Encounters
        if (randomEncounters && GameController.instance.currentMap.randomEncounters.Length > 0)
            CheckRandomEncounter();

        //// Keydown
        //if (InputManager.instance.DownArrow())
        //{
        //    Move(Vector2.down);
        //}
        //else if (InputManager.instance.UpArrow())
        //{
        //    Move(Vector2.up);
        //}
        //if (InputManager.instance.LeftArrow())
        //{
        //    Move(Vector2.left);
        //}
        //else if (InputManager.instance.RightArrow())
        //{
        //    Move(Vector2.right);
        //}

        //// Keyup
        //if (InputManager.instance.DownArrowUp())
        //{
        //    Stop();
        //}
        //else if (InputManager.instance.UpArrowUp())
        //{
        //    Stop();
        //}
        //if (InputManager.instance.LeftArrowUp())
        //{
        //    Stop();
        //}
        //else if (InputManager.instance.RightArrowUp())
        //{
        //    Stop();
        //}
    }

    public void CheckRandomEncounter()
    {
        if ((transform.position - lastCheckedPos4RandEncounters).magnitude > 3)
        {
            if (Random.Range(0, 100) < GameController.instance.currentMap.randomEncountersRate)
            {
                Jrpg.Log("Triggering random battle");
                GameController.instance.StartCoroutine(GameController.instance.TriggerBattle(GameController.instance.currentMap.ChooseRandomEncounter(), "Random"));
            }
            lastCheckedPos4RandEncounters = transform.position;
        }
    }

    // Callback to update party with selection cache
    public void PartySelectionCallback()
    {
        GameController.instance.partyPrefabs.Clear();
        foreach(Battler b in GameController.instance.selectionCache)
        {
            GameController.instance.partyPrefabs.Add((HeroBattler)b);
        }

        // Check if at least 3 else add random
        int count = GameController.instance.partyPrefabs.Count;
        while (count < 3)
        {
            foreach (HeroBattler h in GameController.instance.unlockedHeroes)
                if (!GameController.instance.partyPrefabs.Contains(h))
                    GameController.instance.partyPrefabs.Add(h);
            count = GameController.instance.partyPrefabs.Count;
        }
    }
}
