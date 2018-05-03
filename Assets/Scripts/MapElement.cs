using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapElement : JrpgBehaviour
{
    [Header("Map Element")]
    public SpriteRenderer spr;
    public GameController gc;

    // Use this for initialization
    public virtual void Start ()
    {
        spr = gameObject.GetComponent<SpriteRenderer>();
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        Jrpg.AdjustSortingOrder(gameObject);
    }
}
