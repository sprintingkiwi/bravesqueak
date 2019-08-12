using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : JrpgBehaviour
{
    Animator anim;

    [Header("Main Settings")]
    public bool projectile;
    public float speed;
    public Effect AfterEffect;
    //GameController GameController.instance;

    [Header("Advanced Custom Settings")]
    public int customLayer = 0;
    public bool loop;
    public bool fade;
    public float fadeSpeed = 0.5f;

    // Use this for initialization
    public virtual void Start ()
    {
        //GameController.instance = GameObject.Find("Game Controller").GetComponent<GameController>();

        anim = gameObject.GetComponent<Animator>();

        // Set effect layer
        if (customLayer == 0)
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 130;
        else
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = customLayer;

        // Fade Effect
        if (projectile || fade)
            Jrpg.Fade(gameObject, 1, fadeSpeed);        
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        if (!projectile && !loop)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
            {
                Debug.Log(name + " has finished");
                Destroy(gameObject);
            }
        }
    }

    // Only for Projectile effects
    public virtual IEnumerator ReachTarget(Vector3 hitHook)
    {
        while (Vector3.Distance(transform.position, hitHook) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, hitHook, speed * Time.deltaTime);
            yield return null;
        }

        yield return Jrpg.Fade(gameObject, 0, 0.2f, true);
    }
}
