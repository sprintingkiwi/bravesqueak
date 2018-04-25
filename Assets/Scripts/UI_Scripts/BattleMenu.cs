using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BattleMenu : MonoBehaviour
{
    BattleController bc;
    InputManager inputManager;
    AudioSource audioSource;

    [Header("Battlers")]
    public string phase;
    //public List<HeroBattler> instParty = new List<HeroBattler>();
    //public List<EnemyBattler> instEnemies = new List<EnemyBattler>();
    public HeroBattler playerBattler;
    public List<Battler> legalTargets = new List<Battler>();
    public List<Battler> selectedTargets = new List<Battler>();
    public Battler.Faction selectedFaction;
    public Skill selectedSkill;
    public int currentlySelectedTarget;

    [Header("UI")]
    public GameObject icon;
    public GameObject confirmBtnPrefab;
    public GameObject undoBtnPrefab;
    public GameObject targetCursorPrefab;
    public GameObject areaSelectionPrefab;
    public GameObject skillSelectorPrefab;
    public Effect activeBattler;
    GameObject targetCursor;
    GameObject areaSelection;

    [Header("Wheel")]
    public string rotation;
    public Vector3 targetAngle;
    public Vector3 currentAngle;
    public float wheelSpeed = 2f;
    public float currentRounded;
    public float targetRounded;
    public float rotationStartTime;
    //public Dictionary<string, Vector3> skillPositions = new Dictionary<string, Vector3>();
    //public Dictionary<string, int> wheelSkillIndex = new Dictionary<string, int>();
    public Dictionary<int, SpriteRenderer> wheelIcons = new Dictionary<int, SpriteRenderer>();
    public List<Vector3> wheelPlaces = new List<Vector3>();
    public int wheelActualPlace;
    public int skillIndex;
    public SkillWheelIcon skillWheelIconPrefab;
    public GameObject skillScroll;

    // Use this for initialization
    public void Start ()
    {
        
    }

    public Coroutine Setup()
    {
        bc = GameObject.Find("Battle Controller").GetComponent<BattleController>();
        inputManager = GameObject.Find("Input Manager").GetComponent<InputManager>();
        rotation = "None";
        audioSource = gameObject.GetComponent<AudioSource>();

        // Populate wheel places list
        wheelPlaces.Add(new Vector3(0, 3.5f, 0));
        wheelPlaces.Add(new Vector3(2.5f, 2.5f, 0));
        wheelPlaces.Add(new Vector3(3.5f, 0, 0));
        wheelPlaces.Add(new Vector3(2.5f, -2.5f, 0));
        wheelPlaces.Add(new Vector3(0, -2.5f, 0));
        wheelPlaces.Add(new Vector3(-2.5f, -2.5f, 0));
        wheelPlaces.Add(new Vector3(-3.5f, 0, 0));
        wheelPlaces.Add(new Vector3(-2.5f, 2.5f, 0));

        // Setup wheel skills and icons
        SetupWheelIcons();
        skillIndex = 2;

        // Setup Active-Battler effect
        Instantiate(activeBattler, playerBattler.transform.Find("Highlighter Hook").position, Quaternion.identity, playerBattler.transform).name = "Highlighter";

        // GUI Elements
        GameObject.Find("Canvas").transform.Find("Skill Scroll Name").gameObject.SetActive(true);

        return StartCoroutine(ProcessChoice());
    }

    // Update is called once per frame
    void Update ()
    {
        
    }

    void RotateWheel()
    {
        // Right wheel rotation
        if (inputManager.RightArrowDown() && rotation == "None" && skillIndex < 4)
        {
            //UpdateWheel("Right");
            if (playerBattler.skills[skillIndex + 1] == null)
                return;
            skillIndex += 1;

            SetSkillScroll();

            rotation = "Right";
            targetAngle = new Vector3(0, 0, transform.eulerAngles.z + 15);
            rotationStartTime = Time.time;
        }
        if (rotation == "Right")
        {
            currentAngle = new Vector3(0, 0, Mathf.LerpAngle(currentAngle.z, targetAngle.z, Time.deltaTime * wheelSpeed));
            transform.eulerAngles = currentAngle;
        }

        // Left wheel rotation
        if (inputManager.LeftArrowDown() && rotation == "None" && skillIndex > 0)
        {
            //UpdateWheel("Left");
            if (playerBattler.skills[skillIndex - 1] == null)
                return;
            skillIndex -= 1;

            SetSkillScroll();

            rotation = "Left";
            targetAngle = new Vector3(0, 0, transform.eulerAngles.z - 15);
            rotationStartTime = Time.time;
        }
        if (rotation == "Left")
        {
            currentAngle = new Vector3(0, 0, Mathf.LerpAngle(currentAngle.z, targetAngle.z, Time.deltaTime * wheelSpeed));
            transform.eulerAngles = currentAngle;
        }

        // Check if rotation finished
        if (rotation != "None" && Time.time - rotationStartTime > 0.8f)
        {            
            rotation = "None";
        }
    }

    void SetSkillScroll()
    {
        // Skill name scrolls
        Skill focusedSkill = playerBattler.skills[skillIndex];
        Jrpg.Log(focusedSkill.name);

        if (skillScroll != null)
            Destroy(skillScroll);
        skillScroll = Instantiate(Resources.Load("SkillScrolls/Scroll_" + focusedSkill.element.ToString()) as GameObject);

        Text skillScrollName = GameObject.Find("Canvas").transform.Find("Skill Scroll Name").GetComponent<Text>();
        skillScrollName.text = focusedSkill.name;
        skillScrollName.transform.position = Camera.main.WorldToScreenPoint(skillScroll.transform.position);
    }

    void SetupWheelIcons()
    {
        for (int i = 0; i < playerBattler.skills.Length; i++)
        {
            if (playerBattler.skills[i] != null)
            {
                SkillWheelIcon wi = Instantiate(skillWheelIconPrefab, transform);
                wi.linkedSkill = playerBattler.skills[i];
                SpriteRenderer spr = wi.GetComponent<SpriteRenderer>();
                spr.sprite = wi.linkedSkill.GetComponent<SpriteRenderer>().sprite;
                spr.color = new Color(1, 1, 1, 0);
                wi.transform.position = transform.Find("Icon Hooks").GetChild(i).position;
                wi.Setup();
            }            
        }

        skillIndex = 2;        

        //// Instance icons on the left side of the wheel
        //if (playerBattler.skills.Count > 3)
        //{
        //    for (int i = 0; i < 2; i++)
        //    {
        //        GameObject skillPreview = Instantiate(icon, transform);
        //        skillPreview.GetComponent<SpriteRenderer>().sprite = playerBattler.skills[playerBattler.skills.Count - 1 - i].icon;
        //        skillPreview.transform.localPosition = wheelPlaces[7 - i];
        //        wheelIcons.Add((7 - i), skillPreview.GetComponent<SpriteRenderer>());
        //    }
        //}

        //// Instance icons on the right side of the wheel
        //for (int i = 0; i < 6; i++)
        //{
        //    GameObject skillPreview = Instantiate(icon, transform);
        //    if (i < playerBattler.skills.Count)
        //        skillPreview.GetComponent<SpriteRenderer>().sprite = playerBattler.skills[i].icon;
        //    skillPreview.transform.localPosition = wheelPlaces[i];
        //    wheelIcons.Add((i), skillPreview.GetComponent<SpriteRenderer>());
        //}
    }

    void moveSkillSelector()
    {

    }

    // Deprecated
    void UpdateWheel(string direction)
    {
        int nextRightPlace = 0;
        int nextRightSkillIndex = 0;

        switch (direction)
        {
            case "Right":
                // Increment skill index
                if (skillIndex < playerBattler.skills.Length - 1)
                    skillIndex += 1;
                else
                    skillIndex = 0;
                // Increment wheel actual place
                if (wheelActualPlace < 7)
                    wheelActualPlace += 1;
                else
                    wheelActualPlace = 0;

                // Instance image for the new right skill icon
                nextRightPlace = wheelActualPlace + 2;
                if (nextRightPlace > 7)
                    nextRightPlace -= 8;
                nextRightSkillIndex = skillIndex + 2;
                if (nextRightSkillIndex > playerBattler.skills.Length - 1)
                    nextRightSkillIndex -= playerBattler.skills.Length;
                wheelIcons[nextRightPlace].sprite = playerBattler.skills[nextRightSkillIndex].GetComponent<SpriteRenderer>().sprite;

                break;

            case "Left":

                // Decrement skill index
                if (skillIndex > 0)
                    skillIndex -= 1;
                else
                    skillIndex = playerBattler.skills.Length - 1;
                // Decrement wheel actual place
                if (wheelActualPlace > 0)
                    wheelActualPlace -= 1;
                else
                    wheelActualPlace = 7;
                // Instance new left skill icon image
                nextRightPlace = wheelActualPlace - 2;
                if (nextRightPlace < 0)
                    nextRightPlace += 6;
                nextRightSkillIndex = skillIndex - 2;
                if (nextRightSkillIndex < 0)
                    nextRightSkillIndex += (playerBattler.skills.Length - 2);
                wheelIcons[nextRightPlace].sprite = playerBattler.skills[nextRightSkillIndex].GetComponent<SpriteRenderer>().sprite;

                break;
        }
    }

    IEnumerator ProcessChoice()
    {
        // Skill selection
        Debug.Log("Waiting action selection");

        yield return Jrpg.Fade(gameObject, 1, 0.5f);

        // Mobile Flow
        if (Jrpg.CheckPlatform() == "Mobile")
        {
            phase = "Skill Selection";

            GameButton confirmButton = Instantiate(confirmBtnPrefab, transform).GetComponent<GameButton>();
            confirmButton.name = "ButtonA";

            selectedSkill = playerBattler.skills[skillIndex];
            Skill lastSelectedSkill = null;

            GameObject skillSelector = Instantiate(skillSelectorPrefab, bc.mainCamera.transform);

            // Label to restart target selection when skill is changed
            MobileSelection:
            Debug.Log("Skill and Target(s) selection on touchscreen");

            SetSkillScroll();

            // Destroy old UI if exist
            if (targetCursor != null)
                Destroy(targetCursor);
            if (areaSelection != null)
                Destroy(areaSelection);

            selectedTargets.Clear();
            currentlySelectedTarget = 0;

            lastSelectedSkill = selectedSkill;

            Debug.Log("Finding legal targets based on selected skill targetType");
            legalTargets.Clear();
            legalTargets = selectedSkill.FindLegalTargets(playerBattler, selectedSkill, bc.enemies.ToArray(), bc.party.ToArray());

            if (selectedSkill.scope != Skill.Scope.Area)
            {
                // Cursor
                CreateTargetCursor();
                MoveTargetCursor();

                Debug.Log("Waiting for confirm on touch button");

                phase = "Target Selection";
                while (!inputManager.ButtonAUp())
                {
                    // Restart selection if skill changed
                    if (selectedSkill != lastSelectedSkill)
                    {
                        yield return null;
                        goto MobileSelection;
                    }
                    yield return null;
                }

                ConfirmTarget();
                Destroy(targetCursor.gameObject);
            }
            else
            {
                // For Area skills only
                selectedFaction = Battler.Faction.Enemies;
                Battler.Faction lastFaction = selectedFaction;
                areaSelection = Instantiate(areaSelectionPrefab, bc.mainCamera.transform) as GameObject;
                yield return Jrpg.Fade(areaSelection, 1, 0.2f);

                phase = "Area Selection";
                // Waiting for confirm
                while (!inputManager.ButtonAUp())
                {
                    // Restart selection if skill changed
                    if (selectedSkill != lastSelectedSkill)
                    {
                        yield return null;
                        goto MobileSelection;
                    }
                    else if (selectedFaction != lastFaction)
                    {
                        yield return StartCoroutine(MoveAreaSelection(selectedFaction));
                        lastFaction = selectedFaction;
                    }

                    // Switching Area with the touchscreen
                    //if (Input.touchCount > 0)
                    //{
                    //    // Checking the side of the touch
                    //    if (Input.GetTouch(0).position.x > 0)
                    //        newFaction = Battler.Faction.Heroes;
                    //    else
                    //        newFaction = Battler.Faction.Enemies;

                    //    if (newFaction != selectedFaction)
                    //    {
                    //        // Update faction
                    //        selectedFaction = newFaction;

                    //        // Move Area Selection sprite to the new Area
                    //        Vector3 targetAreaPos = new Vector3(-areaSelection.transform.position.x, areaSelection.transform.position.y, areaSelection.transform.position.z);
                    //        while (areaSelection.transform.position != targetAreaPos)
                    //        {
                    //            areaSelection.transform.position = Vector3.MoveTowards(areaSelection.transform.position, targetAreaPos, 50f * Time.deltaTime);
                    //            yield return null;
                    //        }
                    //    }
                    //}

                    yield return null;
                }
                // Select all targets in the Area
                ConfirmAreaSelection();
                yield return Jrpg.Fade(areaSelection, 0, 0.2f);
                Destroy(areaSelection);
            }

            phase = "Skill Execution";
            Debug.Log("Action confirmed");

            Coroutine[] cc = new Coroutine[] { Jrpg.Fade(skillSelector, 0, 0.5f, true), Jrpg.Fade(gameObject, 0, 0.5f) };
            foreach (Coroutine c in cc)
                yield return c;
        }

        // Keyboard-Controller Flow
        else
        {
            OtherPlatformSelection:

            SetSkillScroll();

            // Wait until a skill is selected
            while (selectedSkill == null)
            {
                RotateWheel();
                // Confirm for Keyboard / Controllers
                if (inputManager.ButtonADown() && Jrpg.CheckPlatform() == "Other")
                {
                    if (playerBattler.skills[skillIndex].ProcessRequirements(playerBattler))
                        selectedSkill = playerBattler.skills[skillIndex];
                    else
                        Jrpg.PlaySound("Forbidden");
                }
                yield return null;
            }
            Debug.Log("Skill " + selectedSkill.name + " selected");

            yield return Jrpg.Fade(gameObject, 0, 0.5f);

            GameObject undoButton = Instantiate(undoBtnPrefab, bc.mainCamera.transform) as GameObject;
            undoButton.name = "Undo Button";

            // Target Selection
            Debug.Log("Finding legal targets based on selected skill targetType");

            if (skillScroll != null)
                Destroy(skillScroll);

            legalTargets = selectedSkill.FindLegalTargets(playerBattler, selectedSkill, bc.enemies.ToArray(), bc.party.ToArray());

            // Only for non-Area skills
            if (selectedSkill.scope != Skill.Scope.Area)
            {
                phase = "Target Selection";

                // Cursor
                CreateTargetCursor();

                // Move the cursor on currently selected target
                MoveTargetCursor();

                // User input for target selection
                Debug.Log("Waiting target(s) selection");
                while (selectedTargets.Count < selectedSkill.targetsNumber)
                {
                    // Switch between legal targets
                    if (inputManager.RightArrowDown())
                    {
                        if (currentlySelectedTarget < legalTargets.Count - 1)
                            currentlySelectedTarget += 1;
                        else
                            currentlySelectedTarget = 0;

                        MoveTargetCursor();
                    }
                    if (inputManager.LeftArrowDown())
                    {
                        if (currentlySelectedTarget > 0)
                            currentlySelectedTarget -= 1;
                        else
                            currentlySelectedTarget = legalTargets.Count - 1;

                        MoveTargetCursor();
                    }

                    // Button B
                    if (inputManager.ButtonBUp())
                    {
                        Debug.Log("Aborting");
                        phase = "Skill Selection";

                        Destroy(targetCursor.gameObject);
                        Destroy(undoButton);
                        if (selectedSkill.targetsNumber > 1)
                            Destroy(GameObject.Find("Confirm Cursor"));
                        selectedSkill = null;
                        selectedTargets.Clear();
                        yield return Jrpg.Fade(gameObject, 1, 0.5f);
                        goto OtherPlatformSelection;
                    }
                    else
                    {
                        // Button A to confirm target (double touch only on mobile)
                        if (inputManager.ButtonADown())
                        {                            
                            ConfirmTarget();
                        }
                    }

                    yield return null;
                }
            }
            else // Only for Area skills
            {
                phase = "Area Selection";

                // Area skills stuff here...
                if (selectedSkill.GetComponent<AttackSkill>() != null)
                    selectedFaction = Battler.Faction.Enemies;
                else
                    selectedFaction = Battler.Faction.Heroes;
                areaSelection = Instantiate(areaSelectionPrefab, bc.mainCamera.transform) as GameObject;
                areaSelection.transform.position = new Vector3(12.5f * (float)selectedFaction, -10f, 0);
                yield return Jrpg.Fade(areaSelection, 1, 0.2f);
                // Waiting for confirm
                while (!inputManager.ButtonADown())
                {
                    // Switching Area with the arrows
                    if (inputManager.RightArrowDown() || inputManager.LeftArrowDown())
                    {
                        // Invert Area Selection
                        if (selectedFaction == Battler.Faction.Enemies)
                            selectedFaction = Battler.Faction.Heroes;
                        else
                            selectedFaction = Battler.Faction.Enemies;

                        // Move Area Selection sprite to the new Area
                        yield return StartCoroutine(MoveAreaSelection(selectedFaction));
                        //Vector3 targetAreaPos = new Vector3(-areaSelection.transform.position.x, areaSelection.transform.position.y, areaSelection.transform.position.z);
                        //while (areaSelection.transform.position != targetAreaPos)
                        //{
                        //    areaSelection.transform.position = Vector3.MoveTowards(areaSelection.transform.position, targetAreaPos, 50f * Time.deltaTime);
                        //    yield return null;
                        //}
                    }

                    yield return null;
                }
                // Select all targets in the Area
                ConfirmAreaSelection();
                yield return Jrpg.Fade(areaSelection, 0, 0.2f);
                Destroy(areaSelection);
            }

            Destroy(undoButton);
        }

        // Skill Execution
        //playerBattler.UseSkill(selectedSkill, selectedTargets);
        bc.actionsQueue.Add(new BattleAction { user = playerBattler, skill = selectedSkill, targets = selectedTargets });

        // End turn
        //Jrpg.Fade(playerBattler.transform.Find("Highlighter").gameObject, 0, 0.5f, true);
        Destroy(playerBattler.transform.Find("Highlighter").gameObject);
        if (bc.mainCamera.transform.Find("Target Cursor") != null)
            Destroy(bc.mainCamera.transform.Find("Target Cursor").gameObject);
            //Jrpg.Fade(bc.mainCamera.transform.Find("Target Cursor").gameObject, 0, 0.1f, true);
        Destroy(gameObject);
    }

    public void CreateTargetCursor()
    {
        Debug.Log("Creating cursor");
        targetCursor = Instantiate(targetCursorPrefab, Vector3.zero, Quaternion.identity, bc.mainCamera.transform) as GameObject;
        targetCursor.name = "Target Cursor";
        currentlySelectedTarget = 0;
    }

    public void MoveTargetCursor()
    {
        Vector3 cursorPos = new Vector3(legalTargets[currentlySelectedTarget].transform.position.x - 1, legalTargets[currentlySelectedTarget].transform.position.y + 5, 0);
        //TargetCursor targetCursor = bc.mainCamera.transform.Find("Target Cursor").GetComponent<TargetCursor>();
        targetCursor.transform.position = cursorPos;
        Jrpg.Log(legalTargets[currentlySelectedTarget].name);
    }

    void ConfirmTarget()
    {
        selectedTargets.Add(legalTargets[currentlySelectedTarget]);

        // deprecated...
        if (selectedSkill.targetsNumber > 1 && legalTargets.Where(s => s != null && s.faction == selectedTargets[0].faction).Count() > 1)
            ProcessMultitarget();
        else
            selectedSkill.targetsNumber = 1;
    }

    // deprecated...
    void ProcessMultitarget()
    {
        Jrpg.PlaySound("Confirm");
        GameObject c = Instantiate(Resources.Load("ConfirmCursor"), legalTargets[currentlySelectedTarget].transform.position + new Vector3(0, 5f, 0), Quaternion.identity, transform) as GameObject;
        c.name = "Confirm Cursor";
        legalTargets.Remove(legalTargets[currentlySelectedTarget]);
        MoveTargetCursor();
    }

    void ConfirmAreaSelection()
    {
        selectedTargets.Clear();
        foreach (Battler b in legalTargets)
        {
            if (b.faction == selectedFaction)
            {
                selectedTargets.Add(b);
            }
        }
    }

    public IEnumerator MoveAreaSelection(Battler.Faction faction)
    {
        Vector3 targetAreaPos = new Vector3(12.5f * (float)faction, areaSelection.transform.position.y, areaSelection.transform.position.z);
        while (areaSelection.transform.position != targetAreaPos)
        {
            areaSelection.transform.position = Vector3.MoveTowards(areaSelection.transform.position, targetAreaPos, 50f * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDestroy()
    {
        // Remove skill scrolls stuff
        if (GameObject.Find("Canvas").transform.Find("Skill Scroll Name") != null)
        {
            GameObject.Find("Canvas").transform.Find("Skill Scroll Name").gameObject.SetActive(false);
        }
        if (skillScroll != null)
            Destroy(skillScroll);
    }
}
