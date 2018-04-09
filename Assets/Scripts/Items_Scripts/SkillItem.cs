using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    SpriteRenderer spr;
    public Skill linkedSkill;

	// Use this for initialization
	public void Setup ()
    {
        spr = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
