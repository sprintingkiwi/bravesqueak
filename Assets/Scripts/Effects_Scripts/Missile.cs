using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Effect
{
    // This is an override made to custom the projectile movement towards target
    public override IEnumerator ReachTarget(Vector3 hitHook)
    {
        // this must be changed...
        while (Vector3.Distance(transform.position, hitHook) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, hitHook, speed * Time.deltaTime);
            yield return null;
        }

        Jrpg.Fade(gameObject, 0, 0.2f, true);
    }
}
