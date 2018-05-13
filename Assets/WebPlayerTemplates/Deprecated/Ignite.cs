using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ignite : Skill
{
    public override IEnumerator OngoingEffect(Battler target)
    {
        yield return StartCoroutine(base.OngoingEffect(target));

        yield return StartCoroutine(Effect(target));
    }
}
