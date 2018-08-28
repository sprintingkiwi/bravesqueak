using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : AttackSkill
{
    public override int Damage(Battler target)
    {
        int damage = Jrpg.Roll(user.attack, powerRoll);

        if (fightOutcomes[target] == "Parry")
            damage = 0;
            //damage -= (Jrpg.Roll(target.defense) / 4);

        return damage;
    }
}