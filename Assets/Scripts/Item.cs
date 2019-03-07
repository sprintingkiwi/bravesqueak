using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [TextArea(5, 10)]
    public string description;

    // Use this for initialization
    public virtual void Start ()
    {
		
	}

    // Update is called once per frame
    public virtual void Update()
    {
		
	}

    public virtual IEnumerator Fall()
    {
        // Item fall down code


        yield return null;
    }
    
    public virtual IEnumerator Shake(int width = 20, float speed = 0.01f, float pause = 0.5f)
    {
        Debug.Log("Shaking " + name);

        Vector2 dir = new Vector2(Random.Range(-100, 100), Random.Range(-100, 100)).normalized;

        for (int i = 0; i < width; i++)
        {
            transform.Translate(dir * speed);
            yield return null;
        }
        for (int i = 0; i < width; i++)
        {
            transform.Translate(-dir * speed);
            yield return null;
        }

        yield return new WaitForSeconds(pause);
        StartCoroutine(Shake(width));
    }
}
