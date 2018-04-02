using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortedEffect : Effect
{
    //public Skill skill;
    //public Battler.Faction faction;

    public override void Start()
    {
        base.Start();

        Jrpg.AdjustSortingOrder(gameObject);
    }

    public override void Update()
    {

    }
}
