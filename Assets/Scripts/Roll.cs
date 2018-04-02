using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Roll
{
    [System.Serializable]
    public class Dice
    {
        [Range(0, 20)]
        public int faces;
    }

    public Dice[] dices;

    public virtual int Execute()
    {
        int result = 0;

        foreach (Dice dice in dices)
        {
            result = result + Jrpg.RollDice(1, dice.faces);
        }

        return result;
    }
}
