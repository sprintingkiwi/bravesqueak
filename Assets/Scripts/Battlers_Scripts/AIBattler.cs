using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBattler : Battler
{
    public string prefabName;
    public Strategy strategy;

    public override void Setup()
    {
        base.Setup();

        // Here could be placed a check for a "JOBS" container for different sprites or animators
                       
    }

    public virtual void ApplyStrategy()
    {
        Debug.Log("Executing " + gameObject.name + " Combat Strategy");

        //BattleController bc = GameObject.Find("Battle Controller").GetComponent<BattleController>();
        //List<EnemyBattler> instEnemies = bc.instEnemies;
        //List<HeroBattler> instParty = bc.instParty;

        // Strategy
        Strategy s = Instantiate(strategy, transform) as Strategy;
        s.Execute(this);
    }
}
