using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderbolt : Skill
{
    public override int Damage(Battler target)
    {
        return roll.Execute() + Jrpg.Modifier(user.specialAttack);
    }
}
