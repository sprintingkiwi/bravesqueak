//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerController : Character
//{
//    Animator anim;

//	// Use this for initialization
//	public override void Start ()
//    {
//        base.Start();

//        anim = gameObject.GetComponent<Animator>();
//	}
	
//	// Update is called once per frame
//	public override void Update ()
//    {
//        base.Update();

//        // Keydown
//		if (Input.GetKeyDown(KeyCode.DownArrow))
//        {
//            rb.velocity = new Vector2(rb.velocity.x, -speed);
//            anim.SetTrigger("WalkDown");
//        }
//        if (Input.GetKeyDown(KeyCode.UpArrow))
//        {
//            rb.velocity = new Vector2(rb.velocity.x, speed);
//            anim.SetTrigger("WalkUp");
//        }
//        if (Input.GetKeyDown(KeyCode.RightArrow))
//        {
//            rb.velocity = new Vector2(speed, rb.velocity.y);
//            anim.SetTrigger("WalkRight");
//        }
//        if (Input.GetKeyDown(KeyCode.LeftArrow))
//        {
//            rb.velocity = new Vector2(-speed, rb.velocity.y);
//            anim.SetTrigger("WalkLeft");
//        }

//        // Keyup
//        if (Input.GetKeyUp(KeyCode.DownArrow))
//        {
//            rb.velocity = new Vector2(rb.velocity.x, 0);
//            anim.SetTrigger("IdleDown");
//        }
//        if (Input.GetKeyUp(KeyCode.UpArrow))
//        {
//            rb.velocity = new Vector2(rb.velocity.x, 0);
//            anim.SetTrigger("IdleUp");
//        }
//        if (Input.GetKeyUp(KeyCode.RightArrow))
//        {
//            rb.velocity = new Vector2(0, rb.velocity.y);
//            anim.SetTrigger("IdleRight");
//        }
//        if (Input.GetKeyUp(KeyCode.LeftArrow))
//        {
//            rb.velocity = new Vector2(0, rb.velocity.y);
//            anim.SetTrigger("IdleLeft");
//        }
//    }
//}
