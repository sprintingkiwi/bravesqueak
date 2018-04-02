using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bash : MeleeAttack
{
    // Override the way the user moves toward the target
    public override IEnumerator MoveToTarget(Battler target)
    {
        // this must be removed and changed... (for example making the user "jump" on the target)
        return base.MoveToTarget(target);
    }
}