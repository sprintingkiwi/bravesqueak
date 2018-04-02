using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultStrategy : Strategy
{
    public override void Execute(Battler user)
    {
        base.Execute(user);

        ActDefault();
    }
}
