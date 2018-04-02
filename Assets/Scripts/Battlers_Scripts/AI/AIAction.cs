using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AIAction : System.Object
{
    public Skill skill;
    public int weight;
    public ActionCondition[] conditions;
}
