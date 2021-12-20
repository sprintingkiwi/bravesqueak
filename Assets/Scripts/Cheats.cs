using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : MonoBehaviour
{
    public Encounter[] testEncounters;

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
    public string droppedItemClass;

    public bool mute;

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
        GameController.Instance = GameObject.Find("Game Controller").GetComponent<GameController>();

        // Shaking for mobile
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isEditor) return;

        if (Input.GetKeyDown(KeyCode.T))
            GameController.Instance.StartCoroutine(GameController.Instance.TriggerBattle(GameController.Instance.currentMap.ChooseRandomEncounter(), "Random"));

        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(GameController.Instance.TriggerBattle(testEncounters[0], "Random"));

        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(GameController.Instance.TriggerBattle(testEncounters[1], "Random"));

        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine(GameController.Instance.TriggerBattle(testEncounters[2], "Random"));

        if (Input.GetKeyDown(KeyCode.Alpha4))
            StartCoroutine(GameController.Instance.TriggerBattle(testEncounters[3], "Random"));

        if (Input.GetKeyDown(KeyCode.Y))
            ChangeTeam();

        if (Input.GetKeyDown(KeyCode.O))
            StartCoroutine(Jrpg.LoadScene("MainMenu"));

        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(GameObject.Find("Battle Controller").GetComponent<BattleController>().DropItem(droppedItemClass));
        }

        // Save & load
        if (Input.GetKeyDown(KeyCode.F5))
        {
            StartCoroutine(GameController.Instance.Save(3));
        }
        else if(Input.GetKeyDown(KeyCode.F6))
        {
            StartCoroutine(GameController.Instance.Save(4));
        }
        else if(Input.GetKeyDown(KeyCode.F7))
        {
            StartCoroutine(GameController.Instance.Save(5));
        }
        else if(Input.GetKeyDown(KeyCode.F8))
        {
            StartCoroutine(GameController.Instance.Save(6));
        }
        else if (Input.GetKeyDown(KeyCode.F9))
        {
            StartCoroutine(GameController.Instance.Load(3));
        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {
            StartCoroutine(GameController.Instance.Load(4));
        }
        else if (Input.GetKeyDown(KeyCode.F11))
        {
            StartCoroutine(GameController.Instance.Load(5));
        }
        else if (Input.GetKeyDown(KeyCode.F12))
        {
            StartCoroutine(GameController.Instance.Load(6));
        }

        //if (Jrpg.CheckPlatform() == "Mobile")
        //ShakeOnMobile();

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerController pc = GameObject.Find("Player").GetComponent<PlayerController>();
            pc.randomEncounters = !pc.randomEncounters;
            Jrpg.Log("Random Encounters: " + pc.randomEncounters.ToString(), "Build");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (GameController.Instance.music.mute)
            {
                GameController.Instance.music.mute = false;
            }
            else
            {
                GameController.Instance.music.mute = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine(GameController.Instance.ShowBattleTip());
        }
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
    //            StartCoroutine(GameController.instance.Load(0));
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
            GameController.Instance.partyPrefabs[i] = (HeroBattler)teams[actualTeam].memebrs[i];
        }
    }
}
