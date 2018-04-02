using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Jrpg
{    
    public enum WeaponType { Melee, Ranged };
    public enum DamageType { Slashing, Bludgeoning, Piercing };

    [Header("Properties")]
    public WeaponType weaponType;
    public bool twoHanded;
    public bool thrown;    
    public DamageType damageType;
    //public string damageDice;
    public Roll roll;
    // Speed modifier (0 or negative)
    public int speedModifier;
    public SoundEffect[] soundEffects;

    [Header("Skill override")]       
    public bool moveToTarget;
    public float pauseBeforeEffect;
    public Effect targetEffect;
    public Sprite icon;

    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // Apply permanent equip modifiers
    public virtual void ApplyModifiers(Battler user)
    {
        user.deltaStats[user.speed.ID] += speedModifier;
    }

    public virtual void Effect()
    {
        Debug.Log("Processing " + gameObject.name + " weapon effect");
    }

    public virtual int Damage(int userLevel)
    {
        Debug.Log("Processing " + gameObject.name + " weapon damage");
        //int result = 0;
        //string[] rolls = damageDice.Split(';');
        //foreach (string roll in rolls)
        //{
        //    string[] operands = roll.Split('+');
        //    string diceRoll = operands[0];
        //    int mod = 0;
        //    if (operands.Length > 1)
        //        mod = int.Parse(operands[1]);
        //    int dice = int.Parse(diceRoll.Split('d')[0]);
        //    int faces = int.Parse(diceRoll.Split('d')[1]);
        //    result = result + Roll(dice, faces) + mod;
        //}

        int damage = 0;
        int rolls = userLevel / 2;
        for (int i = 0; i < rolls; i++)
        {
            damage = damage + roll.Execute();
        }

        return damage;
    }
}
