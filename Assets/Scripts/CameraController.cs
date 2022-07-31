using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 originalPos;
    public List<Coroutine> activeCoroutines = new List<Coroutine>();

	// Use this for initialization
	public virtual void Start ()
    {
        originalPos = transform.position;
	}

    public virtual void OnDisable()
    {
        foreach (Coroutine c in activeCoroutines)
            StopCoroutine(c);
    }

    public virtual IEnumerator DefaultFollow (Battler actor, float speed=1f, float width=2.5f)
    {
        Debug.Log("Camera is following " + actor.name);

        Vector3 dir = (actor.transform.position - transform.position).normalized;
        Vector3 targetPos = new Vector3(width * dir.x, width * dir.y, transform.position.z);
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("Camera reached follow position for " + actor.name);
    }

    public virtual IEnumerator Return(float speed=1)
    {
        Debug.Log("Camera is returning to original position");

        while (Vector3.Distance(transform.position, originalPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = originalPos;

        Debug.Log("Camera returned to original position");
    }

    public virtual IEnumerator Shake(float shakeDuration=1f, float shakeAmount=0.3f, float decreaseFactor=1.0f)
    {
        while (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
            yield return null;
        }

        shakeDuration = 0f;
        transform.localPosition = originalPos;
    }

    public virtual IEnumerator Focus(Battler target, float speed=2f, float distance=20f)
    {
        Vector3 targetPos = new Vector3(target.transform.position.x, target.transform.position.y, -distance);
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        //StartCoroutine(Return());
    }

    public virtual IEnumerator Move(Vector3 delta, float speed=2f, bool setAsOriginalPosition = false)
    {       
        Vector3 targetPos = transform.position + delta;

        Jrpg.Log("Moving camera to " + targetPos);

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }
        //transform.position = targetPos;
        //StartCoroutine(Return());

        if (setAsOriginalPosition)
            originalPos = transform.position;
    }

    public virtual IEnumerator MoveTo(Vector3 targetPos, float speed = 2f, bool setAsOriginalPosition = false)
    {
        yield return StartCoroutine(Move(targetPos - transform.position, speed, setAsOriginalPosition));
    }

    public virtual IEnumerator SlideTo(Vector3 targetPos, float threshold = 0.5f, float speed = 2f, bool setAsOriginalPosition = false, IEnumerator callback = null)
    {
        Jrpg.Log("Sliding camera to " + targetPos);

        while (Vector3.Distance(transform.position, targetPos) > threshold)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        StartCoroutine(callback);

        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        if (setAsOriginalPosition)
            originalPos = transform.position;
    }
}
