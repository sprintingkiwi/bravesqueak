using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OraProNobis : SupportSkill
{
    public override IEnumerator Effect(Battler target)
    {
        yield return StartCoroutine(base.Effect(target));
        
        // should also resurrect KO party members...
        // ...

        // Heal
        int healValue = Jrpg.Roll(user.support, powerRoll);
        Jrpg.Heal(target, healValue);

        // Cure Status
        foreach (Transform s in target.transform.Find("STATUS"))
            Destroy(s.gameObject);

        yield return null;
    }
}
