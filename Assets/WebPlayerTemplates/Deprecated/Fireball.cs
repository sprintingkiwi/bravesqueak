using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : Skill
{
    public override int Damage(Battler target)
    {
        //int damage = 0;
        // Roll skill damage
        //for (int i = 0; i < user.level / 4; i++)
        //{            
        //}

        return roll.Execute() + Jrpg.Modifier(user.specialAttack);
    }
}
