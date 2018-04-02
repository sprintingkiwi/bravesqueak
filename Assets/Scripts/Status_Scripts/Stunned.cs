using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunned : Status
{
    public override void Effect()
    {
        base.Effect();

        holder.canAct = false;
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
