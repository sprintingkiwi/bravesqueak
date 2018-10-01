using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : AttackSkill
{
    public override int Damage(Battler target)
    {
        int damage = Jrpg.Roll(user.specialAttack, powerRoll);

        if (fightOutcomes[target] == "Parry" && target.parryType == Battler.ParryType.Special)
            damage = 0;
            //damage -= (Jrpg.Roll(target.specialDefense) / 4);

        return damage;
    }
}
