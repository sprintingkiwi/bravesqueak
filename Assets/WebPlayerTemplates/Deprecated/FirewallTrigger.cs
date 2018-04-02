using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallTrigger : ActionTrigger
{
    public Roll damageRoll;
    public Battler firewallTarget;

    public override void Setup()
    {
        base.Setup();


    }

    public override bool CheckTrigger(BattleAction ba)
    {
        if (ba.targets.Contains(firewallTarget) && ba.skill.moveToTarget)
        {
            return true;
        }
        else
            return false;
    }

    public virtual IEnumerator Effect(BattleAction ba)
    {
        Jrpg.Damage(ba.user, damageRoll.Execute());

        yield return null;
    }
}
