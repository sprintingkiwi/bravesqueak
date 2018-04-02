using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    float startTime;
    float t;
    public Image image;
    public float alpha;
    public float speed = 0.5f;

    void Start()
    {
        image = gameObject.GetComponent<Image>();
        image.enabled = true;

        startTime = Time.time;
        t = 0;
    }

    void Update()
    {
        if (image.color.a != alpha)
        {
            t = (Time.time - startTime) / speed;
            image.color = new Color(0f, 0f, 0f, Mathf.SmoothStep(1, alpha, t));
        }
    }
}
