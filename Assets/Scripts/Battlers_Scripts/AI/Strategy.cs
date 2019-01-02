using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public class Strategy : MonoBehaviour
{
    [Header("System")]
    public BattleController bc;
    public Battler user;

    [System.Serializable]
    public class AIAction : System.Object
    {
        public Skill skill;
        public int weight;
        public ActionCondition[] conditions;
    }
    public AIAction selectedAction;
    public bool conditionCheck;

    [Header("Skills")]
    public AIAction[] actions;    

    public virtual void Start()
    {

    }

    public virtual void Execute(Battler user)
    {
        Jrpg.Log("Processing " + name + " strategy");

        bc = GameObject.Find("Battle Controller").GetComponent<BattleController>();

        this.user = user;
        //instEnemies = bc.instEnemies;
        //instParty = bc.instParty;

        ActDefault();
    }

    public virtual AIAction ChooseAction()
    {
        List<ActionCondition> andConds = new List<ActionCondition>();
        List<ActionCondition> orConds = new List<ActionCondition>();
        List<AIAction> legalActions = new List<AIAction>();

        // For each action, analyze conditions
        foreach (AIAction act in actions)
        {
            // Shield for missing skills in actions list
            if (act.skill == null)
                continue;

            if (act.conditions.Length > 0)
            {
                // Separate and/or type conditions
                foreach (ActionCondition cond in act.conditions)
                {
                    Jrpg.Log("Separating and or type conditions");

                    if (cond.conditionType == "and")
                        andConds.Add(cond);
                    else
                        orConds.Add(cond);
                }

                // and-type conditions
                Jrpg.Log("Looping thround andConditions");
                int andCount = 0;
                foreach (ActionCondition ac in andConds)
                {
                    Jrpg.Log("Evaluating condition: " + ac.conditionFunction);

                    // Evaluate condition
                    EvaluateActionCondition(ac);
                    if (conditionCheck)
                        andCount += 1;
                    conditionCheck = false;
                }
                // Only if all the and conditions are true then add this action to the legal actions list
                if (andCount >= andConds.Count)
                {
                    legalActions.Add(act);
                    foreach (ActionCondition ac in andConds)
                        act.weight += ac.weightInfluence;
                }

                // or-type conditions
                foreach (ActionCondition oc in orConds)
                {
                    Jrpg.Log("Looping thround orConditions");

                    // Evaluate condition
                    EvaluateActionCondition(oc);
                    // If the condition results true and this action is not in the legal actions list then add it
                    if (conditionCheck)
                    {
                        act.weight += oc.weightInfluence;
                        if (!legalActions.Contains(act))
                            legalActions.Add(act);
                    }                        
                    conditionCheck = false;
                }
            }
            else
            {
                Jrpg.Log("Adding unconditioned action " + act.skill.name + " to legal actions");
                legalActions.Add(act);
            }
        }

        // Filter legal actions for their requirements
        Jrpg.Log("Legal actions for " + user.name + " :");
        foreach (AIAction la in legalActions.ToArray())
        {
            Jrpg.Log(la.skill.name);
            if (!la.skill.ProcessRequirements(user))
                legalActions.Remove(la);
        }

        // Selection of a legal action
        if (legalActions.Count > 0)
        {           
            // Weighted random selection
            selectedAction = WeightedRandom(legalActions);
            Jrpg.Log(user.name + " selected action " + selectedAction.skill.name);
            return selectedAction;
        }
        // If there are no legal actions avaiable, just wait
        else
        {
            Jrpg.Log("No legal actions for " + user.name + ". Just waiting...");
            AIAction waitAction = new AIAction()
            {
                skill = (Resources.Load("Skills/Wait") as GameObject).GetComponent<Skill>()
            };
            return waitAction;
        }
    }

    void EvaluateActionCondition(ActionCondition condition)
    {
        System.Type thisType = this.GetType();
        MethodInfo condFunc = thisType.GetMethod(condition.conditionFunction);
        Jrpg.Log("Trying to invoke " + condFunc.Name);

        // Collect parameters from action condition
        List<object> paramList = new List<object>();
        foreach (int p in condition.condParameters)
            paramList.Add(p);
        object[] parameters = paramList.ToArray();

        // Invoke condition function passing condition parameters
        condFunc.Invoke(this, parameters);
        Jrpg.Log("Condition check: " + conditionCheck);
    }

    AIAction WeightedRandom(List<AIAction> legalActions)
    {
        Jrpg.Log("Weighted random selection of a legal action");

        // Calculate sum of weigths
        int sumOfWeights = 0;
        foreach (AIAction a in legalActions)
            sumOfWeights += a.weight;

        // Take random number greater than 0 and less than sum of weights
        int rnd = Random.Range(0, sumOfWeights);

        // Log
        foreach (AIAction a in legalActions)
            Jrpg.Log(a.skill.name);

        // Algorithm
        foreach (AIAction a in legalActions)
        {
            if (rnd < a.weight)
                return a;
            rnd -= a.weight;
        }
        Jrpg.Log("Should never execute this");
        return null;
    }

    public static List<Battler> ChooseRandomTarget(Battler user, Skill selectedSkill, Battler[] enemies, Battler[] party)
    {
        // Correct skill target type for enemies strategies, otherwise enemies could often choose to attack their mates
        if (selectedSkill.scope == Skill.Scope.All || selectedSkill.scope == Skill.Scope.Others)
            selectedSkill.scope = Skill.Scope.Enemies;

        List<Battler> legalTargets = selectedSkill.FindLegalTargets(user, selectedSkill, enemies, party);
        List<Battler> selectedTargets = new List<Battler>();

        foreach (Battler lt in legalTargets)
            Jrpg.Log("Legal target: " + lt.name);

        if (selectedSkill.scope == Skill.Scope.Area)
        {
            if (selectedSkill.GetComponent<SupportSkill>() != null)
            {
                selectedSkill.targetedArea = Battler.Faction.Enemies;
                foreach (Battler b in enemies.Concat(party))
                    if (b.faction == user.faction)
                        selectedTargets.Add(b);
            }
            else
            {
                selectedSkill.targetedArea = Battler.Faction.Heroes;
                foreach (Battler b in enemies.Concat(party))
                    if (b.faction != user.faction)
                        selectedTargets.Add(b);
            }

            Jrpg.Log(user.name + " selected " + selectedSkill.targetedArea.ToString() + " Area as target");
        }
        else
        {
            for (int i = 0; i < selectedSkill.targetsNumber; i++)
            {
                selectedTargets.Add(legalTargets[Random.Range(0, legalTargets.Count)]);

                Jrpg.Log(user.name + " selected " + selectedTargets[i].name + " as target");

                legalTargets.Remove(selectedTargets[i]);
            }
        }

        return selectedTargets;
    }

    public virtual void ActDefault()
    {
        Skill selectedSkill = ChooseAction().skill;

        //user.UseSkill(selectedSkill, ChooseRandomTarget(selectedSkill));
        bc.actionsQueue.Add(new BattleController.BattleAction { user = this.user, skillPrefab = selectedSkill, targets = ChooseRandomTarget(user, selectedSkill, bc.enemies.ToArray(), bc.party.ToArray()) });

        Destroy(gameObject, 2f);
    }

    // Actions Conditions
    public void AlwaysTrue()
    {
        conditionCheck = true;

        Jrpg.Log("Finished to evaluate condition Always True");
    }

    public void HitPoints(int percentage)
    {
        if (user.hitPoints < ((user.maxHP.value * percentage) / 100))
            conditionCheck = true;

        Jrpg.Log("Finished to evaluate condition for Hit Points");
    }

    public void TestCondition()
    {
        if (bc.turnNumber < 3)
            conditionCheck = true;

        Jrpg.Log("Finished to evaluate condition Test");
    }
}
