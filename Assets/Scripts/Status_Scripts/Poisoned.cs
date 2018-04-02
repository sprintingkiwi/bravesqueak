using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poisoned : Status
{
    public override void Effect()
    {
        base.Effect();

        Jrpg.Damage(holder, Mathf.RoundToInt(holder.maxHP.value * 0.2f), Skill.Element.Earth);
    }

    public override void SaveRoll()
    {
        base.SaveRoll();


    }

    public override void CheckDestroyed()
    {
        base.CheckDestroyed();


    }
}
