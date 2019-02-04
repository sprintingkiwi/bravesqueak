using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMouseFeedback : MonoBehaviour {

    public float enlargeFactor;
    List<Coroutine> enlargingCoroutines = new List<Coroutine>();
    Vector3 targetScale;
    Vector3 standardScale;

    void Awake()
    {
        standardScale = transform.localScale;
    }

    void OnEnable()
    {
        transform.localScale = standardScale;
    }

    void OnDisable()
    {
        foreach (Coroutine c in enlargingCoroutines)
            StopCoroutine(c);
        targetScale = standardScale;
    }

    void OnMouseEnter()
    {
        StartEnlargeCoroutine(enlargeFactor);
    }

    void OnMouseExit()
    {
        StartEnlargeCoroutine(1/enlargeFactor);
    }

    void StartEnlargeCoroutine(float factor)
    {
        if (enlargingCoroutines.Count > 0)
        {
            foreach (Coroutine c in enlargingCoroutines)
                StopCoroutine(c);
            transform.localScale = targetScale;
        }
            
        enlargingCoroutines.Add(StartCoroutine(Enlarge(factor)));
    }

    IEnumerator Enlarge(float factor)
    {
        targetScale = transform.localScale * factor;
        float direction = Mathf.Sign(targetScale.x - transform.localScale.x);
        while (Mathf.Abs(transform.localScale.x - targetScale.x) > 0.05f)
        {
            transform.localScale = new Vector3(transform.localScale.x + (0.01f * direction), transform.localScale.y + (0.01f * direction), transform.localScale.z);
            yield return null;
        }
        transform.localScale = targetScale;

        yield return null;
    }
}
