using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseSP : SupportSkill
{
    public override IEnumerator Effect(Battler target)
    {
        base.Effect(target);

        yield return null;
    }
}
