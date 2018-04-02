using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSP : Perk
{
    public override void Setup(Battler holder)
    {
        base.Setup(holder);

        holder.skillPoints += Random.Range(3, 6);
    }
}
