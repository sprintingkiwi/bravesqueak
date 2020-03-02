using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWall : WallSkill
{
    [Header("Fire Wall")]
    public Effect burnEffect;

    public override IEnumerator WallEffect(Skill receivedSkill)
    {
        yield return StartCoroutine(base.WallEffect(receivedSkill));

        Instantiate(burnEffect, receivedSkill.user.transform.position, Quaternion.identity, receivedSkill.user.transform);
        Jrpg.Damage(receivedSkill.user, Jrpg.Roll(user.specialAttack, powerRoll), element);
        if (receivedSkill.user.hitPoints <= 0)
        { // Needs to be fixed if battlers should die from wall damage
            //StopCoroutine(receivedSkill.flowCoroutine);
            //yield return StartCoroutine(ProcessDeath(receivedSkill.user));
        }

        yield return null;
    }
}
