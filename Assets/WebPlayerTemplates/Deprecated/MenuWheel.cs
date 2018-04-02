using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWheel : JrpgBehaviour
{
    public InputManager inputManager;

    public HeroBattler playerBattler;
    public Skill selectedSkill;

    public string rotation;
    public Vector3 targetAngle;
    public Vector3 currentAngle;
    public float wheelSpeed = 2f;
    public float currentRounded;
    public float targetRounded;
    public float rotationStartTime;

    public GameObject icon;

    public Dictionary<string, Vector3> skillPositions = new Dictionary<string, Vector3>();
    public Dictionary<string, int> wheelSkillIndex = new Dictionary<string, int>();

    // Use this for initialization
    void Start ()
    {
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();

        rotation = "None";

        skillPositions.Add("Previous", new Vector3(-2.5f, 2.5f, 0));
        skillPositions.Add("Selected", new Vector3(0, 3.5f, 0));
        skillPositions.Add("Following", new Vector3(2.5f, 2.5f, 0));

        wheelSkillIndex.Add("Previous", (playerBattler.skills.Count - 1));
        wheelSkillIndex.Add("Selected", 0);
        wheelSkillIndex.Add("Following", 1);

        // Setup wheel skills and icons
        SetupWheelIcons();
    }
	
	// Update is called once per frame
	void Update ()
    {
        RotateWheel();
	}

    void SetupWheelIcons()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject skillPreview = Instantiate(icon, transform);
            string pos = "";
            switch (i)
            {
                case 0:
                    pos = "Selected";
                    break;
                case 1:
                    pos = "Following";
                    break;
                case 2:
                    pos = "Previous";
                    break;
            }
            skillPreview.GetComponent<SpriteRenderer>().sprite = playerBattler.skills[wheelSkillIndex[pos]].icon;
            skillPreview.transform.localPosition = skillPositions[pos];
        }
    }

    void RotateWheel()
    {
        // Right wheel rotation
        if (inputManager.RightArrowDown() && rotation == "None")
        {
            Debug.Log("Rotating wheel counter-clockwise");
            rotation = "Right";
            targetAngle = new Vector3(0, 0, transform.eulerAngles.z + 45);
            rotationStartTime = Time.time;
            UpdateWheelIndex("Right");

        }
        if (rotation == "Right")
        {
            currentAngle = new Vector3(0, 0, Mathf.LerpAngle(currentAngle.z, targetAngle.z, Time.deltaTime * wheelSpeed));
            transform.eulerAngles = currentAngle;
        }

        // Left wheel rotation
        if (Input.GetButtonDown("Left") && rotation == "None")
        {
            rotation = "Left";
            targetAngle = new Vector3(0, 0, transform.eulerAngles.z - 45);
            rotationStartTime = Time.time;
            UpdateWheelIndex("Left");

        }
        if (rotation == "Left")
        {
            currentAngle = new Vector3(0, 0, Mathf.LerpAngle(currentAngle.z, targetAngle.z, Time.deltaTime * wheelSpeed));
            transform.eulerAngles = currentAngle;
        }

        // Check if rotation finished
        if (Time.time - rotationStartTime > 1)
            rotation = "None";
        //currentRounded = Mathf.Round(currentAngle.z);
        //targetRounded = Mathf.Round(targetAngle.z);
        //if (targetRounded == 360)
        //    targetRounded = 0;
        //if (currentRounded == targetRounded)
        //    rotation = "None";

        if (Input.GetButtonDown("ButtonA"))
            selectedSkill = playerBattler.skills[wheelSkillIndex["Selected"]];
    }

    void UpdateWheelIndex(string direction)
    {
        switch (direction)
        {
            case "Right":
                wheelSkillIndex["Selected"] += 1;
                wheelSkillIndex["Following"] += 1;
                if (wheelSkillIndex["Previous"] < playerBattler.skills.Count - 1)
                    wheelSkillIndex["Previous"] += 1;
                else
                    wheelSkillIndex["Previous"] = 0;
                break;
            case "Left":
                wheelSkillIndex["Previous"] -= 1;
                wheelSkillIndex["Following"] -= 1;
                if (wheelSkillIndex["Selected"] > 0)
                    wheelSkillIndex["Selected"] -= 1;
                else
                    wheelSkillIndex["Selected"] = playerBattler.skills.Count - 1;
                break;
        }

        Debug.Log(wheelSkillIndex["Selected"]);

    }
}
