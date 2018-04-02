using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBehavior : MonoBehaviour
{
    public float min;
    public float max;
    public float t;
    private float startTime;
    public SpriteRenderer sprite;
    public string status;
    public bool destroyAfter;

    public virtual void Start()
    {
        startTime = Time.time;
        sprite = gameObject.GetComponent<SpriteRenderer>();

        FadeIn();
    }

    public virtual void Update()
    {
        if (0 <= t && t <= 1)
        {
            Fade(min, max);
        }
        else if (destroyAfter)
            Destroy(gameObject);
    }

    public void Fade(float min = 0, float max = 1, float duration = 0.5f)
    {
        t = (Time.time - startTime) / duration;
        sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(min, max, t));
    }

    public virtual void FadeIn()
    {
        min = 0;
        max = 1;
    }

    public virtual void FadeOut(bool destroyAfter = true)
    {
        min = 1;
        max = 0;

        if (destroyAfter)
            this.destroyAfter = true;
    }    
}
