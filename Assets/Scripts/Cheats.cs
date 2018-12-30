using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    GameController gc;
    public Encounter testEncounter;

    [System.Serializable]
    public class Team
    {
        public Battler[] memebrs = new Battler[3];
    }
    public Team[] teams;
    public int actualTeam = 0;

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

    // Persistence
    public static Cheats cGInstance;
    void Awake()
    {
        // Persistence
        if (cGInstance == null)
        {
            cGInstance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();

        // Shaking for mobile
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GameObject.FindObjectOfType<AudioListener>().name);

        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(gc.TriggerBattle(testEncounter, "Random"));

        if (Input.GetKeyDown(KeyCode.Y))
            ChangeTeam();

        if (Input.GetKeyDown(KeyCode.O))
            StartCoroutine(Jrpg.LoadScene("MainMenu"));

        //if (Input.GetKeyDown(KeyCode.N))
        //StartCoroutine(Jrpg.LoadScene("SkillStore"));

        // Save & load
        if (Input.GetKeyDown(KeyCode.F5))
        {
            StartCoroutine(gc.Save(0));
        }
        else if(Input.GetKeyDown(KeyCode.F6))
        {
            StartCoroutine(gc.Save(1));
        }
        else if(Input.GetKeyDown(KeyCode.F7))
        {
            StartCoroutine(gc.Save(2));
        }
        else if(Input.GetKeyDown(KeyCode.F8))
        {
            StartCoroutine(gc.Save(3));
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            StartCoroutine(gc.Load(0));
        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {
            StartCoroutine(gc.Load(1));
        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            StartCoroutine(gc.Load(2));
        }
        else if (Input.GetKeyDown(KeyCode.F12))
        {
            StartCoroutine(gc.Load(3));
        }

        //if (Jrpg.CheckPlatform() == "Mobile")
            //ShakeOnMobile();
    }

    //void ShakeOnMobile()
    //{
    //    if (Jrpg.CheckPlatform() == "Mobile")
    //    {
    //        Vector3 acceleration = Input.acceleration;
    //        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
    //        Vector3 deltaAcceleration = acceleration - lowPassValue;
    //        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold || Input.GetKeyDown(KeyCode.E))
    //        {
    //            // Perform your "shaking actions" here. If necessary, add suitable
    //            // guards in the if check above to avoid redundant handling during
    //            // the same shake (e.g. a minimum refractory period).
    //            StartCoroutine(gc.Load(0));
    //        }
    //    }        
    //}

    void ChangeTeam()
    {
        Jrpg.Log("Changing heroes team", type: "Build");

        if (actualTeam < teams.Length - 1)
            actualTeam += 1;
        else
            actualTeam = 0;

        for (int i = 0; i < 3; i++)
        {
            gc.partyPrefabs[i] = (HeroBattler)teams[actualTeam].memebrs[i];
        }
    }
}
