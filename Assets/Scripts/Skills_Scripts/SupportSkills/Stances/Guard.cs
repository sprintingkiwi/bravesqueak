using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : SupportSkill
{
    public override IEnumerator Effect(Battler target)
    {
        yield return StartCoroutine(base.Effect(target));

        if (!target.tags.Contains("Guard"))
            target.tags.Add("Guard");
    }

    public override void PostEffect(Battler target)
    {
        base.PostEffect(target);

        if (target.tags.Contains("Guard"))
            target.tags.Remove("Guard");
    }
}
