using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionCondition : System.Object
{
    public string conditionFunction;    
    public string conditionType = "and";
    public int weightInfluence;

    [System.Serializable]
    public class ConditionParameter : System.Object
    {
        public int intParameter;
        public string stringParameter;
    }
    public int[] condParameters;
}
