using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunned : Status
{
    public override IEnumerator Effect()
    {
        yield return StartCoroutine(base.Effect());

        holder.canAct = false;
        yield return null;
    }

    public override void CheckDestroyed()
    {
        base.CheckDestroyed();


    }
}
