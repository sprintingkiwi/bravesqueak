using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBlade : NPC
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        transform.rotation *= Quaternion.Euler(0, 0, Random.Range(-10, 10));
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        transform.rotation = Quaternion.identity;
    }
}
