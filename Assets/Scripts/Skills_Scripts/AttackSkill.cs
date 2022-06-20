using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AttackSkill : Skill
{
    // Temporary modifiers for attack attempt. Could be influenced by perks or ongoing skills.
    //public int attackMod;
    //public int defenseMod;
    //public int specialAttackMod;
    //public int specialDefenseMod;
    //public int dodgeMod;
    //public int criticalMod;
    //public float damageMod;
    [System.Serializable]
    public class Modifier
    {
        public string name;
        public float value;
    }
    public Modifier[] mods = new Modifier[]
    {
        new Modifier { name = "ATK" },
        new Modifier { name = "SAT" },
        new Modifier { name = "DEF" },
        new Modifier { name = "SDF" },
        new Modifier { name = "EVA" },
        new Modifier { name = "CRT" },
        new Modifier { name = "DMG" }
    };
    public float GetMod(string name)
    {
        foreach (Modifier mod in mods)
            if (mod.name == name)
                return mod.value;
        return 0;
    }
    public void SetMod(string name, float value)
    {
        foreach (Modifier mod in mods)
            if (mod.name == name)
                mod.value = value;
    }

    public override IEnumerator Fighting()
    {
        // Trigger user animation
        yield return StartCoroutine(Jrpg.PlayAnimation(user, userAnimation, true));

        if (scope == Scope.Area && areaEffect != null)
            //Jrpg.PlayEffect(Vector3.zero, areaEffect, Battler.Faction.Heroes);
            yield return StartCoroutine(Jrpg.PlayEffect(new Vector3(10f * (float)targets[0].faction, -7f, 0), areaEffect, user.faction, areaEffect.waitComplete));

        // Process first execution effect
        yield return StartCoroutine(ProcessEffects(Effect));
    }

    public override IEnumerator ProcessEffects(Func<Battler, IEnumerator> effectFunction)
    {
        List<Coroutine> fightCoroutines = new List<Coroutine>();

        foreach (Battler target in targets.ToArray())
        {
            yield return StartCoroutine(effectFunction(target));

            fightCoroutines.Add(StartCoroutine(ProcessFightOutcome(target)));
        }

        foreach (Coroutine c in fightCoroutines.ToArray())
            yield return c;

        yield return new WaitForSeconds(0.5f);
        foreach (Battler target in targets.ToArray())
            LogOutcomes(target);
        yield return new WaitForSeconds(0.5f);
    }

    public override IEnumerator Effect(Battler target)
    {
        yield return StartCoroutine(base.Effect(target));

        yield return StartCoroutine(DefaultAttack(target));

        yield return null;
    }

    public override IEnumerator OngoingEffect(Battler target)
    {
        yield return StartCoroutine(base.OngoingEffect(target));

        yield return StartCoroutine(DefaultAttack(target));

        yield return null;
    }    

    public virtual IEnumerator DefaultAttack(Battler target)
    {
        if (scope == Scope.Area)
            fightOutcomes[target] = "Success";
        else if (fightOutcomes[target] == "")
            fightOutcomes[target] = AttackAttempt(target);

        if (fightOutcomes[target] == "Success")
        {
            // Calculate skill damage with multiplier
            Debug.Log("Calculating " + gameObject.name + " damage");
            float dmg = Damage(target);
            float dmgMod = (int)GetMod("DMG");
            if (dmgMod != 0)
            {
                Jrpg.Log("Damage multiplied by " + dmgMod.ToString());
                dmg *= dmgMod;
                cameraShake = true;
            }
            damageOutcomes[target] = (int)dmg;            
        }

        yield return null;
    }

    // should be modified to use dodge OR parry
    // This version is only if the target holds the UNTOUCHABLE perk
    public virtual string AttackAttempt(Battler target)
    {
        Jrpg.Log("Processing attack attempt between " + user.name + " and " + target.name);
        

        int attackRoll = Jrpg.RollDice(1, 20);
        int dodgeRoll = 0;
        int defenseRoll = 10;

        // Select right modifier based on attack type
        if (gameObject.GetComponent<MeleeAttack>() != null)
        {
            attackRoll += Jrpg.Roll(user.attack, modifier: accuracy + (int)GetMod("ATK"));
            
            // Parry only if target has a melee parry type
            //if (user.parryType == Battler.ParryType.Melee)
            defenseRoll += Jrpg.Roll(target.defense);
        }
        else
        {
            attackRoll += Jrpg.Roll(user.specialAttack, modifier: accuracy + (int)GetMod("SAT"));
            
            // Parry only if target has a special parry type
            //if (user.parryType == Battler.ParryType.Special)
            defenseRoll += Jrpg.Roll(target.specialDefense);
        }
        dodgeRoll += Jrpg.Roll(target.speed);

        Jrpg.Log(user.name + " rolled " + attackRoll + " vs " + target.name + " dodge: " + dodgeRoll + " and defense: " + defenseRoll, "Visible");
        
        if (attackRoll <= dodgeRoll)
        {
            Debug.Log(target.name + " dodged the attack");
            return "Dodge";
        }
        else if (attackRoll <= defenseRoll)
        {
            Debug.Log(target.name + " parried the attack");
            return "Parry";
        }
        else
        {
            Debug.Log(user.name + " attack succeed");
            return "Success";
        }
    }   

    public virtual int Damage(Battler target)
    {       
        return 0;
    }

    //public virtual int WeaponDamage(Battler target)
    //{
    //    Debug.Log("Calculating base " + user.primaryWeapon.name + " damage");

    //    // Primary weapon damage
    //    int damage = user.primaryWeapon.Damage(user.level);
    //    Weapon.WeaponType wt = user.primaryWeapon.weaponType;
    //    if (wt == Weapon.WeaponType.Melee || (wt == Weapon.WeaponType.Ranged && user.primaryWeapon.thrown))
    //    {
    //        damage += Jrpg.Modifier(user.attack.value);
    //        if (user.primaryWeapon.twoHanded)
    //            damage += Jrpg.Modifier(user.attack.value) / 2;
    //    }

    //    // Secondary weapon damage
    //    if (user.secondaryWeapon != null)
    //    {
    //        wt = user.secondaryWeapon.weaponType;
    //        if (wt == Weapon.WeaponType.Melee || (wt == Weapon.WeaponType.Ranged && user.secondaryWeapon.thrown))
    //            damage += Jrpg.Modifier(user.attack.value) / 2;
    //    }

    //    // Damage cannot be less than 1
    //    if (damage < 1)
    //        damage = 1;

    //    return damage;
    //}

    //public virtual int Formula()
    //{
    //    return 0;
    //}

    public virtual IEnumerator ProcessFightOutcome(Battler target)
    {
        if (targetEffect == null)
            targetEffect = (Resources.Load("EmptyEffect") as GameObject).GetComponent<Effect>();

        // Shoot if effect is a projectile
        if (targetEffect.projectile)
            yield return StartCoroutine(ShootProjectile(target));        

        // Cases for target hit reaction
        switch (fightOutcomes[target])
        {
            case "Success":
                // Shake Camera
                if (cameraShake)
                    bc.cameraCoroutines.Add(StartCoroutine(bc.battleCamera.Shake()));
                //else // Little camera shake also after normal attacks?
                    //bc.cameraCoroutines.Add(StartCoroutine(bc.mainCamera.Shake(0.2f, 0.1f, 1)));
                    
                // If the effect was projectile then play the after effect, otherwise play the normal effect
                if (targetEffect.projectile)
                {
                    Debug.Log("Playing " + name + " after effect");
                    yield return StartCoroutine(Jrpg.PlayEffect(target, targetEffect.AfterEffect));
                }
                else
                    yield return StartCoroutine(Jrpg.PlayEffect(target, targetEffect));

                // Waiting a little
                yield return new WaitForSeconds(0.1f);
                // Trigger target hit animation
                if (scope != Scope.Self)
                {
                    yield return StartCoroutine(Jrpg.PlayAnimation(target, targetAnimation, false));
                    target.PlaySoundEffect(target.hitSound);
                }

                // Modify target hit points
                Jrpg.Damage(target, damageOutcomes[target], element);

                // Status chance
                foreach (StatusChance sc in statusChances)
                {
                    if (target.transform.Find("STATUS").Find(sc.status.name) != null)
                        continue;

                    int chance = UnityEngine.Random.Range(1, 100);
                    if (chance <= sc.chance)
                        yield return StartCoroutine(target.AddStatus(sc.status));
                }

                break;

            case "Parry":
                // Parry target effect and animation
                yield return StartCoroutine(Jrpg.PlayAnimation(target, "parry", false));
                yield return StartCoroutine(Jrpg.PlayEffect(target, target.parryEffect));

                // Modify target hit points
                //if (!target.perfectParry)
                //Jrpg.Damage(target, damageOutcomes[target], element);
                break;

            case "Dodge":
                // Dodge target movement
                target.PlaySoundEffect(target.dodgeSound);
                // Dodge Movement
                target.targetPos = new Vector3(target.transform.position.x + 5f * (float)target.faction, target.transform.position.y, target.transform.position.z);
                Debug.Log(target.name + " dodge go");
                while (target.transform.position != target.targetPos)
                {
                    target.transform.position = Vector3.MoveTowards(target.transform.position, target.targetPos, 50 * Time.deltaTime);
                    yield return null;
                }
                Debug.Log(target.name + " dodge back");
                while (target.transform.position != target.originalPos)
                {
                    target.transform.position = Vector3.MoveTowards(target.transform.position, target.originalPos, 50 * Time.deltaTime);
                    yield return null;
                }
                break;
        }
    }

    public virtual void LogOutcomes(Battler target)
    {
        string log = "";
        switch (fightOutcomes[target])
        {
            case "Success":
                log = target.name + " received " + damageOutcomes[target].ToString() + "  " + element.ToString() + " damage";
                Jrpg.Log(log);
                break;

            case "Parry":
                log = target.name + " parried the attack";
                Jrpg.Log(log);
                break;

            case "Dodge":
                log = target.name + " dodged the attack";
                Jrpg.Log(log);
                break;
        }
    }
}
