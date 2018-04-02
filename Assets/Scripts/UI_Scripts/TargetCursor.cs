using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCursor : MonoBehaviour
{
    [Header("Lerp")]
    // Animate the game object from -1 to +1 and back
    public float minimum = -1.0F;
    public float maximum = 1.0F;
    // Starting value for the Lerp
    static float t = 0.0f;
    // Lerp options
    public Vector3 lerpMultiplier;
    public float speed;

    void Start()
    {

    }

    void Update()
    {
        // animate the position of the game object...
        transform.position = new Vector3(transform.position.x + (Mathf.Lerp(minimum, maximum, t) * lerpMultiplier.x), transform.position.y + (Mathf.Lerp(minimum, maximum, t) * lerpMultiplier.y), transform.position.z + (Mathf.Lerp(minimum, maximum, t) * lerpMultiplier.z));

        // .. and increate the t interpolator
        t += speed * Time.deltaTime;

        // now check if the interpolator has reached 1.0
        // and swap maximum and minimum so game object moves
        // in the opposite direction.
        if (t > 1.0f)
        {
            float temp = maximum;
            maximum = minimum;
            minimum = temp;
            t = 0.0f;
        }
    }
}
