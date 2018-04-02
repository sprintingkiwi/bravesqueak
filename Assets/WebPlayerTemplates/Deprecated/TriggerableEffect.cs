using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerableEffect : MonoBehaviour
{
    public BattleController bc;

	// Use this for initialization
	public virtual void Setup ()
    {
        bc = GameObject.Find("Battle Controller").GetComponent<BattleController>();
	}
}
