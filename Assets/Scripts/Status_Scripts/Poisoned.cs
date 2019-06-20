using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poisoned : Status
{
    public override IEnumerator Effect()
    {
        yield return StartCoroutine(base.Effect());

        Jrpg.Damage(holder, Mathf.RoundToInt(holder.maxHP.value * 0.2f), Skill.Element.Earth);
        yield return null;
    }

    public override void CheckDestroyed()
    {
        base.CheckDestroyed();


    }
}
