using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportSkill : Skill
{
    public override IEnumerator Effect(Battler target)
    {
        yield return StartCoroutine(base.Effect(target));

        fightOutcomes[target] = "Success";
    }
}
