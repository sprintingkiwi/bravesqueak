using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedMapElement : MapElement
{
    [Header("Animated Map Element")]
    public Animator anim;

    // Use this for initialization
    public override void Start ()
    {
        base.Start();

        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    public override void Update ()
    {
        base.Update();
	}
}
