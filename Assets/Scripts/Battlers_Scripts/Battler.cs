using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using UnityEditor;

public class Battler : MonoBehaviour
{
    [TextArea(5, 10)]
    public string description;

    // MAIN STATS
    [Header("Main Stats")]
    public Stat attack = new Stat { ID = "ATK" };
    public Stat defense = new Stat { ID = "DEF" };
    public Stat specialAttack = new Stat { ID = "SAT" };
    public Stat specialDefense = new Stat { ID = "SDF" };
    public Stat support = new Stat { ID = "SUP" };
    //public Stat accuracy = new Stat { ID = "ACC" };
    public Stat speed = new Stat { ID = "SPD" };
    public Stat maxHP = new Stat { ID = "maxHP" };

    // Dictionary reference for string stats ID
    public Dictionary<string, Stat> stats = new Dictionary<string, Stat>();
    // Delta values for stats
    public Dictionary<string, int> deltaStats = new Dictionary<string, int>();
    // Original values for stats
    public Dictionary<string, int> originalStats = new Dictionary<string, int>();
    

    // Other stats
    [Header("Other parameters")]
    public int hitPoints;
    public int skillPoints;
    //public enum ParryType { Melee, Special };
    //public ParryType parryType;
    //public int level;
    //public Stat baseAttackBonus;
    public string characterName;     
    public enum Size { Medium, Colossal, Gargantuan, Huge, Large, Small, Tiny, Diminutive, Fine };
    public Size size;
    public Skill.Element elementAffinity;
    public enum Species { Slime, Undead, Beast, Plant, Insect, Dragon };
    public Species species;
    public Perk[] perks = new Perk[2];
    public bool movable = true;
    public Battler evolution;

    [System.Serializable]
    public class EffectDelay
    {
        public string animation;
        public float lenghtPercentage;
    }
    public EffectDelay[] effectsDelay;

    // Equipment
    //[Header("Equipment")]
    //public Weapon primaryWeapon;
    //public Weapon secondaryWeapon;
    //public string equipmentPreset;    

    // Sounds
    [Header("Effects and sounds Override")]
    public AudioClip hitSound;
    public AudioClip dodgeSound;
    public AudioClip defeatSound;
    // Effects
    public Effect parryEffect;    

    // System
    public Dictionary<Size, int> sizeModifiers = new Dictionary<Size, int>();
    [Header("System")]
    public int sizeModifier;
    public Vector3 originalPos;
    public Vector3 targetPos;
    public Animator anim;
    public SpriteRenderer spr;
    //public PolygonCollider2D col;
    public List<GameObject> targets = new List<GameObject>();
    public List<GameObject> currentlyActiveSkills = new List<GameObject>();
    public Skill currentSkill;
    public bool canAct;
    public bool perfectParry;
    public AudioSource audioSource;
    //public float sideMod;
    //public bool confirmedAsTarget;
    public BattleController bc;
    public Dictionary<Skill, float> warmups = new Dictionary<Skill, float>();
    public Dictionary<Skill, float> cooldowns = new Dictionary<Skill, float>();
    public enum Faction { Enemies = -1, Heroes = 1 };
    public Faction faction;
    public List<WallSkill> firewalls = new List<WallSkill>();
    public BattlerHUD hud;

    // Use this for initialization
    public virtual void Start ()
    {
                
    }

    public virtual void Setup()
    {
        GameController.Instance = GameObject.Find("Game Controller").GetComponent<GameController>();
        bc = GameObject.Find("Battle Controller").GetComponent<BattleController>();

        // Populate stats dictionary
        stats.Add(attack.ID, attack);
        stats.Add(specialAttack.ID, specialAttack);
        stats.Add(specialDefense.ID, specialDefense);
        stats.Add(support.ID, support);
        //stats.Add(accuracy.ID, accuracy);
        stats.Add(speed.ID, speed);
        stats.Add(defense.ID, defense);
        stats.Add(maxHP.ID, maxHP);

        // Populate delta stats dictionary
        foreach (KeyValuePair<string, Stat> s in stats)
        {
            deltaStats.Add(s.Value.ID, 0);
        }

        // Test
        //deltaStats["DEX"] += 5;

        originalPos = transform.position;
        targetPos = transform.position;

        // Populate size modifiers table dictionary
        sizeModifiers.Add(Size.Colossal, -8);
        sizeModifiers.Add(Size.Gargantuan, -4);
        sizeModifiers.Add(Size.Huge, -2);
        sizeModifiers.Add(Size.Large, -1);
        sizeModifiers.Add(Size.Medium, 0);
        sizeModifiers.Add(Size.Small, 1);
        sizeModifiers.Add(Size.Tiny, 2);
        sizeModifiers.Add(Size.Diminutive, 4);
        sizeModifiers.Add(Size.Fine, 8);

        sizeModifier = sizeModifiers[size];

        // Apply Main Stat modifiers
        //...

        // Apply equipment modifiers
        //if (primaryWeapon != null)
        //    primaryWeapon.ApplyModifiers(this);
        //if (secondaryWeapon != null)
        //    secondaryWeapon.ApplyModifiers(this);

        // Apply passive skills modifiers

        // Set anim layer
        //ChangeAnimLayer(equipmentPreset);

        // SOUNDS
        audioSource = gameObject.AddComponent<AudioSource>();
        // Load default sound effects
        if (hitSound == null)
            hitSound = Resources.Load("AudioClips/HitDefault") as AudioClip;
        if (dodgeSound == null)
            dodgeSound = Resources.Load("AudioClips/DodgeDefault") as AudioClip;
        if (defeatSound == null)
            defeatSound = Resources.Load("AudioClips/DefeatDefault") as AudioClip;
        // EFFECTS
        if (parryEffect == null)
            parryEffect = (Resources.Load("Parry") as GameObject).GetComponent<Effect>();
              
        // Save skills warmup in a dictionary of the battler to keep trace of warmup values (because the skill always gets destroyed)
        //WarmupsInit();

        // STATUS
        new GameObject("STATUS").transform.parent = transform;

        // HOOKS
        SetupHooks();

        // HP & SP
        hitPoints = maxHP.value;
        skillPoints = Random.Range(3, 8);

        // Stats
        SaveOriginalStats();
        UpdateStats();

        // Sprite stuff
        //SetupCollider();
        spr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
        UpdateSortingOrder();

        // HUD
        SetupHUD();
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        // Adjust layer sorting order based on y position
        //spr.sortingOrder = -(int)(gameObject.transform.position.y * 10);

    }

    public virtual void SaveOriginalStats()
    {
        foreach (KeyValuePair<string, Stat> s in stats)
        {
            originalStats.Add(s.Value.ID, s.Value.value);
        }
    }

    public virtual void UpdateStats()
    {
        foreach (KeyValuePair<string, int> s in deltaStats)
        {
            stats[s.Key].value = originalStats[s.Key] + s.Value;
        }
    }

    // Called by the battle controller while executing actions during the turn battle
    public virtual Coroutine UseSkill(Skill skill, List<Battler> targets)
    {
        bc.actualSkill = Instantiate(skill, GameController.Instance.battleStuff.transform) as Skill;
        return bc.actualSkill.Execute(this, targets);
    }

    //public virtual void LevelUp()
    //{
    //    Debug.Log(gameObject.name + " is leveling up");
    //}

    //public virtual void MoveToTarget(Battler target)
    //{
    //    Debug.Log(gameObject.name + " is moving toward target " + target.name);
    //    targetPos = target.transform.position;
    //}    

    public virtual IEnumerator AddStatus(Status status)
    {
        Jrpg.Log("Adding status " + status.name + " to " + name);
        Status instStatus = Instantiate(status, transform.Find("STATUS"));
        instStatus.name = status.name;
        yield return StartCoroutine(instStatus.Setup());
    }

    public virtual IEnumerator ProcessStatusEffects()
    {
        foreach (Status status in transform.Find("STATUS").GetComponentsInChildren<Status>())
        {
            if (status.enabled)
                yield return StartCoroutine(status.Effect());
            else // Clean disabled status
                Destroy(status.gameObject, 1);
        }
    }

    public virtual IEnumerator StatusSaveRolls()
    {
        foreach (Status status in transform.Find("STATUS").GetComponentsInChildren<Status>())
        {
            if (status.gameObject != null)
                yield return StartCoroutine(status.SaveRoll());
        }
    }

    public virtual void ChangeAnimLayer(string newLayer)
    {
        for (int i = 0; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }

        anim.SetLayerWeight(anim.GetLayerIndex(newLayer), 10);
    }

    public void PlaySoundEffect(AudioClip se)
    {
        audioSource.clip = se;
        audioSource.Play();
    }

    public virtual void OnMouseDown()
    {
        Debug.Log(name + " was touched");

        if (Jrpg.CheckPlatform() == "Mobile")
        {
            if (bc.battleMenu.phase == "Target Selection")
            {
                Debug.Log("Current target: " + name);

                if (bc.battleMenu.legalTargets.Contains(this))
                {

                    bc.battleMenu.currentlySelectedTarget = bc.battleMenu.legalTargets.IndexOf(this);

                    bc.battleMenu.MoveTargetCursor();
                }
            }
            else if (bc.battleMenu.phase == "Area Selection")
            {
                bc.battleMenu.selectedFaction = faction;
            }
            else
            {
                Debug.Log("Will display info about battler...");
            }
        }        
    }

    //public virtual void WarmupsInit()
    //{
    //    foreach (Skill s in skills)
    //    {
    //        if (s.warmup > 0)
    //            warmups.Add(s, s.warmup);
    //    }
    //}

    public virtual void ProcessWarmupsAndCooldowns()
    {
        Skill[] warmupSkills = warmups.Keys.ToArray();
        foreach (Skill s in warmupSkills)
        {
            if (warmups[s] > 0)
                warmups[s] -= 1;
            else
                warmups.Remove(s);
        }

        Skill[] cooldownSkills = cooldowns.Keys.ToArray();
        foreach (Skill s in cooldownSkills)
        {            
            if (cooldowns[s] > 0)
                cooldowns[s] -= 1;
            else
                cooldowns.Remove(s);
        }
    }

    public void UpdateSortingOrder()
    {
        spr.sortingOrder = -(int)(gameObject.transform.position.y * 10);
    }

    public void SetupHooks()
    {
        Transform hooks;
        if (transform.Find("HOOKS") == null)
        {
            hooks = new GameObject("HOOKS").transform;
            hooks.transform.parent = transform;
            hooks.transform.localPosition = Vector3.zero;
        }
        else
            hooks = transform.Find("HOOKS");

        if (hooks.Find("Attack") == null)
        {
            Transform attackHook = new GameObject("Attack").transform;
            attackHook.transform.parent = hooks;
            attackHook.transform.localPosition = new Vector3(-10f, 0, 0);
        }

        if (hooks.Find("Near") == null)
        {
            Transform nearHook = new GameObject("Near").transform;
            nearHook.transform.parent = hooks;
            nearHook.transform.localPosition = new Vector3(-5f, 0, 0);
        }

        transform.Find("HOOKS").localScale = new Vector3((float)faction, 1, 1);
    }

    // Collider for target touch selection
    public void SetupCollider(Collider2D col)
    {
        col = gameObject.AddComponent<PolygonCollider2D>();
        //col.autoTiling = true;
        col.isTrigger = true;
    }

    public void SetupPerks()
    {
        new GameObject("PERKS").transform.parent = transform;

        foreach (Perk p in perks)
        {
            if (p != null)
            {
                Instantiate(p, transform.Find("PERKS")).Setup(this);
            }
        }
    }

    public bool HasPerk(Perk perkToCheck)
    {
        foreach (Transform t in transform.Find("PERKS"))
            if (t.GetComponent<Perk>() == perkToCheck)
                return true;
        return false;
    }

    public float GetEffectDelay(string anim)
    {
        foreach(EffectDelay ed in effectsDelay)
        {
            if (ed.animation == anim)
                return ed.lenghtPercentage;
        }

        if (anim == "attack")
            return 0.6f;
        else if (anim == "special")
            return 0.8f;

        return 0f;
    }

    public void SetupHUD()
    {
        hud = (Instantiate(Resources.Load("BattlerHUD"), GameObject.Find("Canvas").transform.Find("BATTLE HUD")) as GameObject).GetComponent<BattlerHUD>();
        hud.name = name + "_HUD";
        hud.owner = this;
        if (transform.Find("HUD Hook") != null)
            hud.hudHook = transform.Find("HUD Hook");
        hud.Setup();
    }

    public virtual IEnumerator Blink ()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 2; i++)
        {
            spr.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(0.2f);
            spr.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public virtual IEnumerator MoveTo (Vector3 position, float speed = 100)
    {
        Jrpg.Log(name + " started to move towards position " + position.ToString());
        while (transform.position != position)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, speed * Time.deltaTime);
            UpdateSortingOrder();
            yield return null;
        }
        Jrpg.Log(name + " reached target position " + position.ToString());
    }

    public virtual IEnumerator Eat(Food food)
    {
        foreach (Stat s in food.bonuses)
        {
            stats[s.ID].value += s.value;
            Jrpg.Log("Increased " + name + "'s " + s.ID + " by " + s.value.ToString(), "Visible");
            yield return new WaitForSeconds(1);
        }

        yield return null;
    }

    public virtual void OnDestroy()
    {
        //Destroy(hud.gameObject);
    }
}
