using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customizer : MonoBehaviour
{
    [Header("Custom Behaviour")]
    int customizerPriority = 0;

    public virtual void CustomBehaviour()
    {
        if (Debug.isDebugBuild)
            Debug.Log("Triggered " + name + " general custom behaviour");
    }

    public virtual IEnumerator CustomBehaviour(BattleAction action)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Triggered " + name + " action custom behaviour");

        yield return null;
    }

    public virtual IEnumerator CustomBehaviour(Skill skill)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Triggered " + name + " skill custom behaviour");

        yield return null;
    }

    public virtual IEnumerator CustomBehaviour(Battler battler)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Triggered " + name + " battler custom behaviour");

        yield return null;
    }
}
