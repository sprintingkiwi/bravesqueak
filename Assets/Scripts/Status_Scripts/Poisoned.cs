using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poisoned : Status
{
    [Header("Poison")]
    public float damagePercentage;
    public Skill.Element element;

    public override IEnumerator Effect()
    {
        yield return StartCoroutine(base.Effect());

        Jrpg.Damage(holder, Mathf.RoundToInt(holder.maxHP.value * damagePercentage), element);
        yield return null;
    }

    public override void CheckDestroyed()
    {
        base.CheckDestroyed();


    }
}
