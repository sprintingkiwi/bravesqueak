using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSkill : SupportSkill
{
    [Header("Summon")]
    public Summon Summon;
    public int requiredHP;

    public override IEnumerator ProcessEffects(Func<Battler, IEnumerator> effectFunction)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Summoning " + Summon.name);

        // Find a position for the summoned battler
        Vector3 summonPos;
        try
        {
            summonPos = GameObject.Find("BATTLEBACK").transform.Find("HOOKS").Find(user.faction.ToString()).transform.position;
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString());
            summonPos = new Vector3(5 * (float)user.faction, 0, 0);
        }

        // Create it
        AIBattler aib = Instantiate(Summon, summonPos, Quaternion.identity);
        aib.Setup();

        // Initialize summoned battler
        aib.name = aib.characterName.ToString();
        aib.faction = user.faction;
        aib.transform.localScale = new Vector3(-(float)user.faction, 1, 1);
        if (user.faction == Battler.Faction.Heroes)
            bc.party.Add(aib);
        else
            bc.enemies.Add(aib);

        yield return null;
    }

    public override bool ProcessRequirements(Battler user)
    {
        bool customCheck = false;

        if (user.hitPoints < requiredHP)
        {
            Jrpg.Log(name + " cannot be used: not enough HP");
            customCheck = false;
        }
        else
            customCheck = true;

        return base.ProcessRequirements(user) && customCheck;
    }

    public override IEnumerator ApplyCosts()
    {
        yield return StartCoroutine(base.ApplyCosts());

        user.maxHP.value -= requiredHP;

        yield return null;
    }
}
