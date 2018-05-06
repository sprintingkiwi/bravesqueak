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
            function = Effect
        });
    }

    public override IEnumerator Effect(BattleController bc)
    {
        yield return StartCoroutine(base.Effect(bc));

        // The user must behold this perk
        if (!bc.actualAction.user.perks.Contains(this))
            yield break;

        Skill skill = bc.actualAction.skill;

        // The skill must be an attack
        if (skill.GetComponent<AttackSkill>() == null)
            yield break;

        if (skill.scope != Skill.Scope.Area && skill.targets[0].species == banedSpecies)
        {
            Jrpg.Log("Activating " + name + " effect");
            AttackSkill atkSkill = skill.GetComponent<AttackSkill>();
            atkSkill.SetMod("DMG", 2f);
        }

        yield return null;
    }
}
