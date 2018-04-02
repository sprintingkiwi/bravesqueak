using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTrigger : TriggerableEffect
{
    public virtual bool CheckTrigger(BattleAction ba)
    {


        return false;
    }
}
