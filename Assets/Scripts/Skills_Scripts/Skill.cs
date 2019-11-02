using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Skill : Item
{
    //public enum Type { ATK, MAT, SUP }

    [Header("Properties")]
    public string displayName;
    public Color displayNameColor = new Color(0, 0, 0, 255);
    //public Type type;
    //public string expType = "Common";
    public int speed;
    public int accuracy;
    //public bool weaponDamage;
    //public Roll roll;
    public Roll powerRoll;
    public Buff[] buffs;
    public StatusChance[] statusChances;
    public enum Scope { All, Others, Enemies, Friends, Self, Area };
    public Scope scope;
    public int targetsNumber = 1;
    public int duration;
    public bool active = true;
    public bool inBattle = true;
    public bool inField;
    public enum Element { None, Earth, Water, Wind, Fire, Thunder, Ice, Light, Darkness}
    public Element element;
    public int requiredSP;
    public bool baseSkill;
    public HeroBattler epicSkillOf;
    
    //[System.Serializable]
    //public class ShopRequirement
    //{
    //    public HeroBattler.Job job;
    //    public int exp;        
    //}    
    //[Header("Skill Store")]
    //public ShopRequirement shopRequirement;
    //public int coins;

    // Elements multipliers dictionary  
    public static Dictionary<Element, Dictionary<Element, float>> elementsMultipliers = new Dictionary<Element, Dictionary<Element, float>>()
    {
        {Element.Fire, new Dictionary<Element, float>() {{ Element.Water, 2f }, { Element.Earth, 0.5f }, { Element.Ice, 0.5f }, { Element.Fire, 0.5f } }},
        {Element.Water, new Dictionary<Element, float>() {{ Element.Thunder, 2f }, { Element.Wind, 2f }, { Element.Fire, 0.5f }, { Element.Water, 0.5f } }},
        {Element.Wind, new Dictionary<Element, float>() {{ Element.Earth, 2f }, { Element.Thunder, 2f }, { Element.Water, 0.5f }, { Element.Wind, 0.5f } }},
        {Element.Earth, new Dictionary<Element, float>() {{ Element.Fire, 2f }, { Element.Thunder, 0.1f }, { Element.Water, 0.5f }, { Element.Wind, 0.5f } }},
        {Element.Thunder, new Dictionary<Element, float>() {{ Element.Earth, 2f }, { Element.Thunder, 0.1f } }},
        {Element.Ice, new Dictionary<Element, float>() {{ Element.Fire, 2f } }},
        {Element.Light, new Dictionary<Element, float>() {{ Element.Darkness, 2.5f }, { Element.Light, 0.5f } }},
        {Element.Darkness, new Dictionary<Element, float>() {{ Element.Light, 2.5f }, { Element.Darkness, 0.5f } }}
    };
    //public Sprite storeNameImg;

    // Animations sequence
    [Header("Animations sequence")]
    //public string userBeginAnimation;
    public Effect userBeginEffect;
    //public string targetBeginAnimation;
    //public Effect targetBeginEffect;
    //public string userMoveAnimation;
    //public Effect userMoveEffect;
    public string userAnimation = "attack";
    public string[] alternativeUserAnims;
    //public Effect userEffect;
    public string targetAnimation = "hit";
    //public float pauseBeforeEffect;
    public Effect areaEffect;
    public Effect targetEffect;
    //public Effect targetAfterEffect;
    //public string userEndAnimation;
    //public Effect userEndEffect;
    //public string targetEndAnimation;
    //public Effect targetEndEffect;
    public bool moveToTarget;
    public bool cameraFocus;
    public bool cameraShake;

    // Other
    [Header("System")]
    public Battler user;
    //public Battler target;
    public List<Battler> targets;
    public Battler.Faction targetedArea; // For Ongoing Area skills
    public bool effectStillActive;
    public BattleController bc;
    public Dictionary<Battler, string> fightOutcomes = new Dictionary<Battler, string>();
    public Dictionary<Battler, int> damageOutcomes = new Dictionary<Battler, int>();
    public float userMovementSpeed;

    public virtual void Awake()
    {

    }

    // Use this for initialization
    public override void Start ()
    {
        base.Start();
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public virtual Coroutine Execute(Battler user, List<Battler> targets)
    {
        this.user = user;
        this.targets = targets;

        if (Debug.isDebugBuild)
            Debug.Log("Using skill " + gameObject.name);

        GameController.instance = GameObject.Find("Game Controller").GetComponent<GameController>();
        bc = GameObject.Find("Battle Controller").GetComponent<BattleController>();

        // Movement speed
        userMovementSpeed = Mathf.Abs((user.speed.value * 20));
        // Correct too high movement speed
        if (userMovementSpeed > 200)
            userMovementSpeed = 200;

        // Check for alternative user animations
        if (!user.anim.parameters.Select(o => o.name).ToArray().Contains(userAnimation))
            foreach (string aua in alternativeUserAnims)
                if (user.anim.parameters.Select(o => o.name).ToArray().Contains(aua))
                {
                    userAnimation = aua;
                    break;
                }

        foreach (Battler target in targets)
        {
            fightOutcomes.Add(target, "");
            damageOutcomes.Add(target, 0);
        }

        return StartCoroutine(ExecuteFlow());        
    }

    public virtual IEnumerator ExecuteFlow()
    {
        // Startup
        yield return StartCoroutine(StartupBehaviours());

        CheckDeprecatedTargets();

        // Initial Log
        if (Debug.isDebugBuild)
        {
            Debug.Log("Starting " + gameObject.name + "Coroutine");
            foreach (Battler target in targets.ToArray())
                Debug.Log(user.name + " uses " + gameObject.name.Replace("(Clone)", "") + " on " + target.name);
        }           

        // Apply costs
        yield return StartCoroutine(ApplyCosts());

        // Check if the skill has an override method
        //bool ongoing = GetType().GetMethod("OngoingEffect").DeclaringType == GetType();               

        yield return new WaitForSeconds(0.1f);

        // Trigger begin animations and effects
        yield return StartCoroutine(Beginning());

        // Move user towards target
        if (moveToTarget && user.movable)
            yield return StartCoroutine(MoveToTarget(targets[0]));

        if (cameraFocus)
            bc.cameraCoroutines.Add(StartCoroutine(bc.mainCamera.Focus(targets[UnityEngine.Random.Range(0, targets.Count - 1)])));

        yield return StartCoroutine(Fighting());

        // User returns to original position (also need to be overwritable...)
        if (moveToTarget)
            yield return StartCoroutine(ReturnToOriginalPosition());

        // END OF THINGS
        yield return StartCoroutine(Ending());
        yield return new WaitForSeconds(0.1f);

        // AssignExperience();

        // Death check and animation
        yield return StartCoroutine(ProcessDeath());

        // Skill destruction for skills without duration
        if (duration <= 0)
        {
            yield return StartCoroutine(PostFlow());

            // Finally destroy the skill gameobject
            Destroy(gameObject, 1f);
        }
        // Otherwise add it to the battle controller Ongoing-Skills list
        else if (!bc.ongoingSkills.Contains(this))
        {
            foreach (Battler target in targets.ToArray())
                if (fightOutcomes[target] == "Success")
                {
                    bc.ongoingSkills.Add(this);
                    break;
                }
        }

        yield return null;
    }

    public virtual IEnumerator OngoingFlow()
    {
        CheckDeprecatedTargets();

        if (Debug.isDebugBuild)
        {
            Debug.Log("Starting Ongoing Flow Coroutine");
            foreach (Battler target in targets.ToArray())
                Debug.Log(gameObject.name.Replace("(Clone)", "") + " has effect on " + target.name);
        }            

        yield return StartCoroutine(ProcessEffects(OngoingEffect));

        yield return StartCoroutine(ProcessDeath());

        // Remove Ongoing-Skill if all the targets are dead
        if (targets.Count <= 0)
        {
            bc.ongoingSkills.Remove(this);
            Destroy(gameObject);
        }

        yield return null;
    }

    public virtual IEnumerator PostFlow()
    {
        CheckDeprecatedTargets();

        // Process post effect
        foreach (Battler target in targets.ToArray())
            PostEffect(target);

        yield return null;
    }

    public virtual IEnumerator StartupBehaviours()
    {
        // Action Startup Custom Functions
        yield return bc.RunCustomizers(BattleController.Customizer.When.ActionStart);
    }

    public virtual void CheckDeprecatedTargets()
    {
        foreach (Battler target in targets.ToArray())
            if (target == null)
                targets.Remove(target);
    }

    public virtual IEnumerator ApplyCosts()
    {
        user.skillPoints -= requiredSP;

        yield return null;
    }

    public virtual IEnumerator Beginning()
    {
        yield return null;

        // User Begin Effect
        if (userBeginEffect != null)
            yield return StartCoroutine(Jrpg.PlayEffect(user, userBeginEffect, wait: true));
    }

    public virtual IEnumerator MoveToTarget(Battler target)
    {
        // Get target user position for moving skills
        if (targets.Count > 1)
            user.targetPos = Vector3.zero;
        else
            user.targetPos = target.transform.Find("HOOKS").Find("Attack").position;
        //user.targetPos = new Vector3(target.transform.position.x + 10f * (float)user.faction, target.transform.position.y, target.transform.position.z);

        if (Debug.isDebugBuild)
            Debug.Log(user.name + " starts to move toward target " + target.name + " at speed " + userMovementSpeed.ToString());

        while (user.transform.position != user.targetPos)
        {
            user.transform.position = Vector3.MoveTowards(user.transform.position, user.targetPos, userMovementSpeed * Time.deltaTime);
            user.UpdateSortingOrder();
            yield return null;
        }

        if (Debug.isDebugBuild)
            Debug.Log("Reached the target");

        // Firewalls Check
        yield return StartCoroutine(ProcessWalls());
    }

    public virtual IEnumerator Fighting()
    {
        // Trigger user animation
        yield return StartCoroutine(Jrpg.PlayAnimation(user, userAnimation, true));
       
        // Trigger effect on target
        if (scope == Scope.Area)
        {
            Jrpg.PlayEffect(new Vector3(10f * (float)targets[0].faction, -7f, 0), areaEffect, targets[0].faction);
        }
        else
            yield return StartCoroutine(Jrpg.PlayEffect(targets[0], targetEffect));

        // Process first execution effect
        yield return StartCoroutine(ProcessEffects(Effect));
    }

    public virtual IEnumerator ReturnToOriginalPosition()
    {
        if (Debug.isDebugBuild)
            Debug.Log("User is returning to original position");

        while (user.transform.position != user.originalPos)
        {
            user.transform.position = Vector3.MoveTowards(user.transform.position, user.originalPos, userMovementSpeed * Time.deltaTime);
            user.UpdateSortingOrder();
            yield return null;
        }

        if (Debug.isDebugBuild)
            Debug.Log("User is back to original position");
    }

    public virtual IEnumerator ProcessEffects(Func<Battler, IEnumerator> effectFunction)
    {
        foreach (Battler target in targets.ToArray())
        {
            yield return StartCoroutine(effectFunction(target));
        }

        yield return new WaitForSeconds(0.1f);
    }    

    public virtual IEnumerator ShootProjectile(Battler target)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Firing " + name + " projectile");

        Vector3 hitHook = new Vector3(5f, 0, 0) + target.transform.position;
        Effect e = Instantiate(targetEffect, (user.transform.position + targetEffect.transform.position), Quaternion.identity, transform);
        yield return StartCoroutine(e.ReachTarget(hitHook));
    }

    public virtual IEnumerator Ending()
    {
        // Trigger end animations and effects

        yield return null;
    }

    public virtual IEnumerator Effect(Battler target)
    {
        Jrpg.Log("Processing " + gameObject.name + " skill effect");

        if (buffs.Length > 0)
            Buff(target);

        yield return null;
    }

    //public virtual void AssignExperience()
    //{
    //    // Assign Experience (always for now...)
    //    if (user.GetComponent<HeroBattler>() != null)
    //    {
    //        // Take experience based on the active job of the user during this battle
    //        ps.experience[user.GetComponent<HeroBattler>().job.ToString()] += 5;
    //        foreach (KeyValuePair<string, int> exp in ps.experience)
    //            Debug.Log(exp.Key + ": " + exp.Value.ToString());
    //    }
    //}

    public virtual IEnumerator ProcessDeath()
    {
        // CYCLE for multitarget death
        //for (int i = 0; i < targets.Count; i++)
        foreach (Battler target in targets.ToArray())
        {
            //target = targets[i];

            if (target.hitPoints <= 0)
            {
                Jrpg.Log(target.name + " is K.O.");

                // Play death sound and wait befor destroying the target
                target.PlaySoundEffect(target.defeatSound);
                yield return new WaitForSeconds(target.defeatSound.length);

                // Fade out of dead target
                if (target.evolution == null)
                    yield return Jrpg.Fade(target.gameObject, 0, 1);

                // Remove dead from lists
                //bc.actionQueue.Remove(target);
                targets.Remove(target);
                if (target.gameObject.GetComponent<HeroBattler>() != null)
                    bc.party.Remove(target.gameObject.GetComponent<HeroBattler>());
                else if (target.gameObject.GetComponent<EnemyBattler>() != null)
                    bc.enemies.Remove(target.gameObject.GetComponent<EnemyBattler>());
                else if (target.gameObject.GetComponent<Food>() != null)
                {
                    Jrpg.Log(user.name + " is eating " + target.name, "Visible");
                    yield return StartCoroutine(user.Eat(target.gameObject.GetComponent<Food>()));

                    // ALTERNATIVE
                    //bc.GameController.instance.foods.Add(target.gameObject.GetComponent<Food>());
                    //Jrpg.Log("Added " + target.name + " food to inventory", "Visible");
                    //Destroy(target.GetComponent<Battler>());
                    //yield break;
                }
                else if (target.gameObject.GetComponent<Perk>() != null)
                {
                    Jrpg.Log("Added " + target.name + " to inventory", "Visible");
                    GameController.instance.unlockedPerks.Add((Resources.Load("Perks/" + target.name.Replace("(Clone)", "")) as GameObject).GetComponent<Perk>());
                }
                else
                    Jrpg.Log("Destroyed battler was not inside any list", "Warning");


                // Evolve if it has an evolution, before destroying
                if (target.evolution != null)
                {
                    Jrpg.Log(target.name + " is evolving into " + target.evolution.name, "Visible");

                    Battler evo = Instantiate(target.evolution, target.transform.position, Quaternion.identity);
                    evo.Setup();

                    // Initialize evolution
                    evo.name = target.characterName.ToString();
                    evo.faction = target.faction;
                    evo.transform.localScale = new Vector3(-(float)target.faction, 1, 1);
                    if (target.faction == Battler.Faction.Heroes)
                        bc.party.Add(evo);
                    else
                        bc.enemies.Add(evo);
                }

                // Destroy dead target
                Destroy(target.gameObject);

                // Death effect on target
    
            }
        }

        yield return new WaitForSeconds(1f);
    }

    // Buffs management
    public virtual void Buff(Battler target)
    {
        foreach (Buff buff in buffs)
        {
            buff.rollResult = buff.roll.Execute();
            target.deltaStats[buff.ID] += buff.rollResult;
        }
        target.UpdateStats();
    }
    // This must be called in post effect
    public virtual void Debuff(Battler target)
    {
        foreach (Buff buff in buffs)
        {
            target.deltaStats[buff.ID] -= buff.rollResult;
        }
        target.UpdateStats();
    }

    // Ongoing effect will be called at the beginning of every turn
    // in which the skill is still active
    public virtual IEnumerator OngoingEffect(Battler target)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Processing " + gameObject.name + " ongoing effect");

        yield return null;
    }

    // Post effect will be called after skill duration ended
    // also useful to restore original properties
    public virtual void PostEffect(Battler target)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Processing " + gameObject.name + " post effect");

        Debuff(target);
    }

    public virtual bool ProcessRequirements(Battler user)
    {
        if (user.skillPoints < requiredSP)
        {            
            if (user.faction == Battler.Faction.Heroes)
                Jrpg.Log(name + " cannot be used: not enough SP", "Build");
            else
                Jrpg.Log(name + " cannot be used: not enough SP");
            return false;
        }
        else
        {
            Jrpg.Log(name + " can be used");
            return true;
        }
    }

    public virtual List<Battler> FindLegalTargets(Battler user, Skill selectedSkill, Battler[] enemies, Battler[] party)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Finding legal targets based on selected skill targetType");

        List<Battler> legalTargets = new List<Battler>();

        switch (selectedSkill.scope)
        {
            case Skill.Scope.All:
                foreach (Battler b in enemies)
                    legalTargets.Add(b);
                foreach (Battler b in party)
                    legalTargets.Add(b);
                break;

            case Skill.Scope.Others:
                foreach (Battler e in enemies)
                    if (e != user)
                        legalTargets.Add(e);
                foreach (Battler h in party)
                    if (h != user)
                        legalTargets.Add(h);
                break;

            case Skill.Scope.Enemies:
                if (user.faction == Battler.Faction.Heroes)
                {
                    foreach (Battler e in enemies)
                        legalTargets.Add(e);
                }
                else
                {
                    foreach (Battler h in party)
                        legalTargets.Add(h);
                }
                break;

            case Skill.Scope.Friends:
                if (user.faction == Battler.Faction.Heroes)
                {
                    foreach (Battler h in party)
                        legalTargets.Add(h);
                }
                else
                {
                    foreach (Battler e in enemies)
                        legalTargets.Add(e);
                }
                break;

            case Skill.Scope.Self:
                legalTargets.Add(user);
                break;

            case Skill.Scope.Area:
                foreach (Battler b in enemies)
                    legalTargets.Add(b);
                foreach (Battler b in party)
                    legalTargets.Add(b);
                break;
        }

        return legalTargets;
    }

    public virtual IEnumerator ProcessWalls()
    {
        foreach (Battler target in targets)
        {
            foreach (WallSkill fs in target.firewalls)
                yield return StartCoroutine(fs.WallEffect(this));
        }

        yield return null;
    }
}
