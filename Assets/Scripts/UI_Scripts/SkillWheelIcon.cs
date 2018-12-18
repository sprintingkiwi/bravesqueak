using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillWheelIcon : MonoBehaviour
{
    public Skill linkedSkill;
    BattleMenu bm;

	// Use this for initialization
	public void Setup ()
    {
        bm = transform.parent.GetComponent<BattleMenu>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.eulerAngles = Vector3.up;
	}

    void OnMouseDown()
    {
        Debug.Log(name + " touched");

        if (Jrpg.CheckPlatform() != "Mobile")
            return;

        if (bm.phase == "Target Selection" || bm.phase == "Area Selection")
        {
            // Confirm for Mobile
            if (linkedSkill.ProcessRequirements(bm.playerBattler))
                bm.selectedSkill = linkedSkill;
            else
                Jrpg.PlaySound("Forbidden");
        }
    }
}
