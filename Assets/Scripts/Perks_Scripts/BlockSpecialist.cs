using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlockSpecialist : Perk
{
    public override void Setup(Battler holder)
    {
        base.Setup(holder);

        //holder.deltaStats["SDF"] += (holder.defense.value / 2);
    }

}
