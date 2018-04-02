using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RestoreHP : SupportSkill
{
    public override IEnumerator Effect(Battler target)
    {
        yield return StartCoroutine(base.Effect(target));

        int healValue = Jrpg.Roll(user.support, powerRoll);
        Jrpg.Heal(target, healValue);

        yield return null;
    }

    public override List<Battler> FindLegalTargets(Battler user, Skill selectedSkill, Battler[] enemies, Battler[] party)
    {
        List <Battler> legalTargets = base.FindLegalTargets(user, selectedSkill, enemies, party);

        // If there's at least one friend with some damage then remove undamaged friends from legal targets
        if (legalTargets.Where(s => s != null && s.hitPoints < s.maxHP.value).Count() > 0)
            foreach (Battler lt in legalTargets.ToArray())
                if (lt.hitPoints == lt.maxHP.value)
                    legalTargets.Remove(lt);

        return legalTargets;
    }
}
