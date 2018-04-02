using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sanctuary : WallSkill
{
    public Roll healRoll;

    public override Coroutine Execute(Battler user, List<Battler> targets)
    {
        scope = Scope.Area;

        return base.Execute(user, targets);
    }

    public override IEnumerator OngoingEffect(Battler target)
    {
        int healValue = Jrpg.Roll(user.support, healRoll);
        Jrpg.Heal(target, healValue);

        yield return null;
    }

    public override IEnumerator WallEffect(Skill receivedSkill)
    {
        yield return StartCoroutine(base.WallEffect(receivedSkill));

        if (receivedSkill.user.species == Battler.Species.Undead)
        {
            Jrpg.Damage(receivedSkill.user, Jrpg.Roll(user.support, powerRoll), element);
        }

        yield return null;
    }
}
