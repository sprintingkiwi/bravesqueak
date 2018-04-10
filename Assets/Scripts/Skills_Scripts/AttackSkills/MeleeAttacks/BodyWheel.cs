using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyWheel : MeleeAttack
{
    public override IEnumerator MoveToTarget(Battler target)
    {
        yield return StartCoroutine(Jrpg.PlayAnimation(user, "attack", false));
        StartCoroutine(AsyncMovement());

        Vector3 triggerDamagePos = new Vector3(-12.5f * (float)user.faction, user.transform.position.y, 0);
        while ((user.transform.position - triggerDamagePos).magnitude > 0.5f)
            yield return null;
    }

    IEnumerator AsyncMovement()
    {
        user.targetPos = new Vector3(-40f * (float)user.faction, user.transform.position.y, 0);
        Debug.Log(user.name + " BODY WHEEL toward targets ");
        Debug.Log("User is moving toward target at speed " + userMovementSpeed.ToString());
        yield return new WaitForSeconds(0.7f);
        while (user.transform.position != user.targetPos)
        {
            user.transform.position = Vector3.MoveTowards(user.transform.position, user.targetPos, 80f * Time.deltaTime);
            //Jrpg.AdjustSortingOrder(user.gameObject);
            yield return null;
        }
        Debug.Log("Reached the target");
        user.transform.position = -user.transform.position;

        yield return StartCoroutine(ProcessWalls());
    }
}
