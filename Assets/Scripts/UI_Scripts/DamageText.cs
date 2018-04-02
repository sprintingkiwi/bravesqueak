using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    Rigidbody2D rb;
    public Vector2 jumpForce;
    public string damageValue;
    public float lifeTime;
    float startTime;

	// Use this for initialization
	public void Setup (string damageValue, float lifeTime)
    {
        this.damageValue = damageValue;
        this.lifeTime = lifeTime;

        startTime = Time.time;
        gameObject.GetComponent<Text>().text = damageValue;
        jumpForce = new Vector2(Random.Range(-100, 100), Random.Range(50, 150));
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.AddForce(jumpForce);
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (Time.time - startTime > lifeTime)
        {
            StartCoroutine(Jrpg.TextFade(gameObject, 1, 1, 0, destroyAfter: true));
        }
	}

    //
    void Jump()
    {

    }
}
