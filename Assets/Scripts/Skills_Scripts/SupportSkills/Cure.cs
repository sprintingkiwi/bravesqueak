using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cure : SupportSkill
{
    public override IEnumerator Effect(Battler target)
    {
        yield return StartCoroutine(base.Effect(target));

        foreach (Transform s in target.transform.Find("STATUS"))
            Destroy(s.gameObject);

        yield return null;
    }
}
