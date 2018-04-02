using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    GameController ps;

    public string targetScene;

    // Shaking
    float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the
    // filtered value will converge towards current input sample (and vice versa).
    float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation,
    // or at least according to Brady! ;)
    public float shakeDetectionThreshold;
    float lowPassFilterFactor;
    Vector3 lowPassValue;    

    // Use this for initialization
    void Start()
    {
        ps = GameObject.Find("Game Controller").GetComponent<GameController>();

        // Shaking
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GameObject.FindObjectOfType<AudioListener>().name);


        // Return to Target Scene
        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(Jrpg.LoadScene(targetScene));

        if (Input.GetKeyDown(KeyCode.N))
            StartCoroutine(Jrpg.LoadScene("SkillStore"));

        // Save & load
        if (ps.canSave)
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                ps.Save(ps.actualSlot);
            }
            else if (Input.GetKeyDown(KeyCode.F9))
            {
                ps.Load(ps.actualSlot);
            }
        }

        if (Jrpg.CheckPlatform() == "Mobile")
            ShakeOnMobile();
    }

    void ShakeOnMobile()
    {
        if (Jrpg.CheckPlatform() == "Mobile")
        {
            Vector3 acceleration = Input.acceleration;
            lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
            Vector3 deltaAcceleration = acceleration - lowPassValue;
            if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold || Input.GetKeyDown(KeyCode.E))
            {
                // Perform your "shaking actions" here. If necessary, add suitable
                // guards in the if check above to avoid redundant handling during
                // the same shake (e.g. a minimum refractory period).
                ps.Load(ps.actualSlot);
            }
        }        
    }

    void LoadTargetScene()
    {
        Debug.Log("Loading cheats target scene: " + targetScene);

        //LoadingStance();
        //SceneManager.LoadScene(targetScene);
        StartCoroutine(Jrpg.LoadScene(targetScene));
    }
}
