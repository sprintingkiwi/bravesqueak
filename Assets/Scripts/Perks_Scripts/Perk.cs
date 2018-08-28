using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Perks are like passive skills. A battler may choose up to 2 (to be tested) perks to equip (just like the max of 4 skills)
// Perks-giving items are dropped by bosses or mini-bosses.
public class Perk : Item
{   
    [TextArea(5, 10)]
    public string description;

    [Header("System")]
    public BattleController bc;
    public Battler holder;

    // Use this for initialization
    public virtual void Setup (Battler holder)
    {
        bc = GameObject.Find("Battle Controller").GetComponent<BattleController>();
        this.holder = holder;

        Debug.Log(name + " perk activated for " + holder.name);
	}

    // Customizer effect. I pass the battle controller as parameter so I can access the actual state of battle and actions
    public virtual IEnumerator CustomEffect(BattleController bc)
    {
        Jrpg.Log("Activated custom effect: " + name);
        yield return null;
    }
}
