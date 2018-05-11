using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bane : Perk
{
    [Header("Bane")]
    public Battler.Species banedSpecies;

    public override void Setup(Battler holder)
    {
        base.Setup(holder);

        // Subscribe Bane Effect to the customizers list in the Battle Controller
        bc.customizers.Add(new BattleController.Customizer()
        {
            when = BattleController.Customizer.When.ActionStart,
            function = CustomEffect,
            source = this
        });
    }

    public override IEnumerator CustomEffect(BattleController bc)
    {
        yield return StartCoroutine(base.CustomEffect(bc));

        // The user must behold this perk
        if (!bc.actualSkill.user.HasPerk(this))
            yield break;

        // The skill must be an attack
        if (bc.actualSkill.GetComponent<AttackSkill>() == null)
            yield break;

        if (bc.actualSkill.scope != Skill.Scope.Area && bc.actualSkill.targets[0].species == banedSpecies)
        {
            Jrpg.Log("Activating " + name + " effect");
            AttackSkill atkSkill = bc.actualSkill.GetComponent<AttackSkill>();
            atkSkill.SetMod("DMG", 2f);
        }

        yield return null;
    }
}
