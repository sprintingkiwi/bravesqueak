using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseAccuracy : Perk
{
    public override void Setup(Battler holder)
    {
        base.Setup(holder);

        bc.customizers.Add(new BattleController.Customizer()
        {
            when = BattleController.Customizer.When.ActionStart,
            function = Effect
        });
    }

    // Just a test... can be removed...
    public override IEnumerator Effect(BattleController bc)
    {           

        yield return null;
    }
}
