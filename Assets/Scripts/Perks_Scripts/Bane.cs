using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bane : Perk
{
    [Header("Bane")]
    public Battler.Species banedSpecies;

    public override void Setup(Battler holder)
    {
        base.Setup(holder);

        holder.skillCustomizers.Add(this);
    }

    public override IEnumerator CustomBehaviour(Skill skill)
    {
        yield return StartCoroutine(base.CustomBehaviour(skill));

        if (skill.GetComponent<AttackSkill>() != null)
        {
            if (skill.scope != Skill.Scope.Area && skill.targets[0].species == banedSpecies)
            {
                AttackSkill atkSkill = skill.GetComponent<AttackSkill>();
                atkSkill.attackMod += 50;
                atkSkill.attackMod += 50;                    
            }            
        }
    }
}
