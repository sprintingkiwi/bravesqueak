using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleAction
{
    public Battler user;
    public Skill skill;
    public List<Battler> targets = new List<Battler>();
}
