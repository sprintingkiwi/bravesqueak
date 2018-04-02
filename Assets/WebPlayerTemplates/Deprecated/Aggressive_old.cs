using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aggressive_old : EnemyCharacter
{
    public Character target;

    // Use this for initialization
    public override void Start ()
    {
        base.Start();

        target = GameObject.Find("Player").GetComponent<PlayerController>();
        level = DetermineEncounterLevel();
	}
	
	// Update is called once per frame
	public override void Update ()
    {
        base.Update();

        if (target.level < level)
            MoveTowardTarget();
        else if (target.level - level > 5)
            MoveTowardTarget(-1);
    }

    int DetermineEncounterLevel()
    {
        int sum = 0;
        foreach (Encounter.Enemy ee in encounter.enemies)
        {
            sum += ee.recipe.level;
        }
        return sum / encounter.enemies.Length;
    }

    void MoveTowardTarget(int dirMod = 1)
    {
        Vector3 targetDirection = (target.transform.position - transform.position).normalized;
        Move(targetDirection * dirMod);
    }
}
