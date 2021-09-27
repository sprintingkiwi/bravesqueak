using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Character
{
    public PlayerController player;

    public override void Start()
    {
        base.Start();

        player = GameController.Instance.player;
    }
}
