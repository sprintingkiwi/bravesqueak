using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : AnimatedMapElement
{
    public float speed;
    public bool mirrorHorizontal = true;
    public int level;

    [Header("System")]
    public Rigidbody2D rb;
    public bool canMove = true;
    //public string direction = "Down";
    public string status = "Idle";
    public Vector2 direction = Vector2.down;
    //public Vector2 heading;
    public Dictionary<Vector2, int> directions = new Dictionary<Vector2, int>();

    // Use this for initialization
    public override void Start ()
    {
        base.Start();

        rb = gameObject.GetComponent<Rigidbody2D>();

        // Setting correspondence between movement vectors and animations
        directions.Add(Vector2.down, 0);
        directions.Add(Vector2.up, 2);
        directions.Add(Vector2.left, 1);
        directions.Add(Vector2.right, 3);
        // Now setting a forfait for the diagonals (for example when you press both down and right arrow, what animation should it play?)
        directions.Add(new Vector2(1, 1), 2);
        directions.Add(new Vector2(1, -1), 0);
        directions.Add(new Vector2(-1, 1), 2);
        directions.Add(new Vector2(-1, -1), 0);
    }
	
	// Update is called once per frame
	public override void Update ()
    {
        base.Update();

        if (status == "Idle" && rb.velocity.magnitude > 0)
            rb.velocity = Vector3.zero;

        // Flip image when heading left
        if (mirrorHorizontal)
        {
            if (direction.x < 0)
                spr.flipX = true;
            else
                spr.flipX = false;
        }           
	}

    public virtual void Move(Vector2 moveDirection)
    {
        Vector2 roundedDir = new Vector2(Mathf.RoundToInt(moveDirection.normalized.x), Mathf.RoundToInt(moveDirection.normalized.y));
        //Debug.Log("rounded direction: " + roundedDir);

        // Animation
        anim.SetInteger("Direction", directions[roundedDir]);
        status = "Walking";

        if (moveDirection != Vector2.zero)
            direction = moveDirection;

        // Velocity
        rb.velocity = (rb.velocity + moveDirection).normalized * speed;
        anim.SetFloat("Speed", speed);        
    }

    public virtual void Stop()
    {
        if (status != "Idle")
        {
            rb.velocity = Vector3.zero;
            anim.SetInteger("Direction", 5);
            status = "Idle";
        }
    }
}
