using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendCharacter : NPC
{
    public override void Update()
    {
        base.Update();

        Move(Vector2.down);
    }
}
