using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : AttackSkill
{
    public override int Damage(Battler target)
    {
        // This way attacks damages are only influenced by skills power rolls
        int damage = Jrpg.Roll(roll: powerRoll);

        // This way attacks damages is influenced by user attack stat as well as by the skill power roll
        //int damage = Jrpg.Roll(user.specialAttack, powerRoll);

        // This way defense will decrease attacks damages
        //if (fightOutcomes[target] == "Parry")
        //damage = 0;
        //damage -= (Jrpg.Roll(target.specialDefense) / 4);

        return damage;
    }
}
