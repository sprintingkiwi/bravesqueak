using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSkill : SupportSkill
{
    [Header("System")]
    public Effect instWallEffect;

    public override IEnumerator Fighting()
    {
        yield return StartCoroutine(Jrpg.PlayAnimation(user, userAnimation, true));

        foreach (Battler target in targets)
        {
            yield return StartCoroutine(Effect(target));
        }
    }

    public override IEnumerator Effect(Battler target)
    {
        if (scope != Scope.Area)
        {
            instWallEffect = Instantiate(targetEffect, target.transform.Find("HOOKS").Find("Near").position, Quaternion.identity, target.transform) as SortedEffect;
        }
        target.firewalls.Add(this);

        yield return null;
    }

    public virtual IEnumerator WallEffect(Skill receivedSkill)
    {
        Debug.Log("Triggered " + name + " firewall effect");

        yield return null;
    }

    public override IEnumerator OngoingFlow()
    {
        yield return null;
    }

    public override IEnumerator PostFlow()
    {
        yield return StartCoroutine(base.PostFlow());

        yield return Jrpg.Fade(instWallEffect.gameObject, 0);
        Destroy(instWallEffect);

        foreach (Battler target in targets)
            target.firewalls.Remove(this);

        yield return null;
    }
}
