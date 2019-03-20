using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MenuButton
{
    GameController gc;

    public void StartNewGame()
    {
        // Choose random starter battlers
        Jrpg.Log("Choosing random starters");
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        gc.partyPrefabs = new List<HeroBattler>();
        for (int i = 0; i < 3; i++)
        {
            while (gc.partyPrefabs.Count < 3)
            {
                HeroBattler b = gc.heroes[Random.Range(0, 7)];
                if (!gc.partyPrefabs.Contains(b))
                {
                    gc.partyPrefabs.Add(b);
                    Jrpg.Log("Added " + b.name);
                }
            }
        }
        if (!gc.unlockAll)
        {
            gc.unlockedHeroes = gc.partyPrefabs.ToArray();
        }
        else
            Jrpg.Log("Unlock all mode: unlocking all characters");

        // Start Everything
        StartCoroutine(Jrpg.JumpAway(GameObject.Find("Title"), Vector3.up));
        StartCoroutine(Jrpg.JumpAway(gameObject, Vector3.down, power: 20f));
        StartCoroutine(Jrpg.LoadScene("World"));
    }
}
