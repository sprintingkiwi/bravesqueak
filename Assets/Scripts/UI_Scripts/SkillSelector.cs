using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSelector : MonoBehaviour
{
    BattleMenu bm;
    Transform iconHooks;

	// Use this for initialization
	void Start ()
    {
        bm = transform.parent.Find("Battle Menu").GetComponent<BattleMenu>();
        iconHooks = bm.transform.Find("Icon Hooks");
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = iconHooks.transform.GetChild(System.Array.IndexOf(bm.playerBattler.skills, bm.selectedSkill)).position;
	}
}
