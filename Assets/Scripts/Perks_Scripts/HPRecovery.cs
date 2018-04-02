using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPRecovery : Perk
{
    public int nextRecoveryTurn;

    public override void Setup(Battler holder)
    {
        base.Setup(holder);

        nextRecoveryTurn = 2;
    }

    void Update()
    {
        if (bc.turnNumber == nextRecoveryTurn)
        {
            Jrpg.Heal(holder, (int)(holder.maxHP.value * 0.2f));
            nextRecoveryTurn += 2;
        }
    }
}
