using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IncreaseAccuracy : Perk
{
    public override void Setup(Battler holder)
    {
        base.Setup(holder);

        //holder.accuracy.value = (int)(holder.accuracy.value * 1.5f);

        bc.actionStartupBehaviours.Add(BuffSkillAccuracy);
    }

    // Just a test... can be removed...
    public IEnumerator BuffSkillAccuracy(BattleController bc)
    {
        Debug.Log("CUSTOM FUNCTION COROUTINE TEST");           

        yield return null;
    }
}
