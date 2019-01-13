using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    Vector3 originalPos;

	// Use this for initialization
	void Start ()
    {
        originalPos = transform.position;

        StartCoroutine(Shake());
	}
	
	// Update is called once per frame
	void Update ()
    {

	}

    // Shake for dropped objects
    public virtual IEnumerator Shake(float shakeDuration = 1f, float shakeAmount = 0.3f, float decreaseFactor = 1.0f)
    {
        while (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
            yield return null;
        }

        shakeDuration = 0f;
        transform.localPosition = originalPos;

        yield return new WaitForSeconds(1);

        StartCoroutine(Shake(Random.Range(0.2f, 0.5f), Random.Range(0.1f, 0.2f), 1.5f));
    }
}
