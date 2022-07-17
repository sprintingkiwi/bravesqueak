using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System;
//using UnityEngine.Networking;
using System.Runtime.InteropServices;

public class Jrpg : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public string[] defeatedBosses;
        public string savedCurrentMapName;
        //public string lastScene;
        public float[] playerPosition = new float[3];
        public List<string> unlockedHeroes = new List<string>();
        public List<string> partyHeroes = new List<string>();
        public List<string> unlockedSkills = new List<string>();
        public List<string> unlockedPerks = new List<string>();
        //public List<string>[] skillLists = new List<string>[] { new List<string>(), new List<string>(), new List<string>() };
        public HeroData[] heroesData = new HeroData[8];
    }

    [System.Serializable]
    public class HeroData
    {
        public string name;
        public string[] equippedSkills = new string[5];
        public string[] equippedPerks = new string [2];
    }

    public static void Log(string text, string type = "", float lifeTime = 10f)
    {
        if (Debug.isDebugBuild || type == "Build")
        {
            if (type == "Warning")
                Debug.LogWarning(text);
            else
                Debug.Log(text);

            if (type == "Visible" || type == "Build")
            {
                // Destroy old log if exists
                GameObject logs = GameObject.Find("LOGS");
                int oldLogs = logs.transform.childCount;
                for (int i = 0; i < oldLogs; i++)
                {
                    Transform t = logs.transform.GetChild(i);
                    t.GetComponent<RectTransform>().Translate(Vector3.up * 40);

                    //Destroy(logs.transform.GetChild(i).gameObject);
                }

                // Instantiate new log
                GameObject log = Instantiate(Resources.Load("JrpgLog"), GameObject.Find("LOGS").transform) as GameObject;
                log.name = "Log " + log.transform.GetSiblingIndex().ToString();

                Text logText = log.transform.Find("Log").GetComponent<Text>();

                // Write text
                logText.text = text;

                // Destroy anyway after a while
                Destroy(log, lifeTime);
            }
        }        
    }

    public static int RollDice(int dice, int faces)
    {
        int result = 0;

        for (int i = 0; i < dice; i++)
        {
            result = result + UnityEngine.Random.Range(1, faces);
        }

        return result;
    }

    public static int Roll(Stat stat = null, Roll roll = null, int modifier = 100)
    {
        int gap = 5;
        int result = 0;

        // Stat value 50 if stat parameter is not gived
        if (stat == null)
            stat = new Stat() { ID = "", value = 50 };

        // D12 is roll parameter is not given
        if (roll == null)
            roll = new Roll() { dices = new Roll.Dice[] { new Roll.Dice() { faces = 12 } } };

        // Rolls
        for (int i = 0; i < stat.value / gap; i++)
            result += roll.Execute();

        // Base Value
        //int baseValue = 3 * (stat.value / (gap * 4));
        //result += baseValue;

        // Percentage Modifier
        result = (result * modifier) / 100;

        return result;
    }

    //public static int Modifier(Stat stat)
    //{
    //    int mod = (stat.value - 10) / 2;
    //    return mod;
    //}    

    public static string CheckPlatform()
    {
        if (Debug.isDebugBuild)
        {
            return GameObject.Find("Game Controller").GetComponent<GameController>().simulatedPlatform.ToString();
        }
        else
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return "Mobile";
            }
            else
            {
                return "Other";
            }
        }                 
    }

    public static void AdjustSortingOrder(GameObject go)
    {
        go.GetComponent<SpriteRenderer>().sortingOrder = -(int)(go.transform.position.y * 10);
    }

    public static Coroutine Fade(GameObject target, float alpha = 1, float speed = 1, bool destroyAfter = false)
    {
        //ps.fadingCoroutinesCount += 1;
        return GameController.Instance.Fade(target, alpha, speed, destroyAfter);
    }

    public static IEnumerator TextFade(GameObject target, float speed = 1f, float min = 0, float max = 1, bool destroyAfter = false)
    {
        //Debug.Log("Fading " + target.name + " from " + min.ToString() + " to " + max.ToString());

        float startTime = Time.time;
        float t = 0;

        while (t < 1)
        {
            t = (Time.time - startTime) / speed;
            Color c = target.GetComponent<Text>().color;
            target.GetComponent<Text>().color = new Color(c.r, c.g, c.b, Mathf.SmoothStep(min, max, t));
            yield return null;
        }

        if (destroyAfter)
            Destroy(target);
    }

    public static IEnumerator LoadScene(string sceneName, Coroutine[] customCoroutines = null)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
               
        // Wait for the game to finish saving, just in case...
        while (GameController.Instance.isSaving)
        {
            yield return null;
        }

        // Loading animation
        GameObject loading = Instantiate(Resources.Load("Loading") as GameObject);
        yield return Fade(loading, 1);

        // Custom coroutines
        if (customCoroutines != null)
            foreach (Coroutine cc in customCoroutines)
                yield return cc;

        while (async.progress < 0.9f)
            yield return null;
        yield return GameController.Instance.StartCoroutine(GameController.Instance.SetVolume(0));

        async.allowSceneActivation = true;
        while (!async.isDone)
            yield return null;
    }

    public static void Damage(Battler target, int value, Skill.Element element, float lifeTime=0.5f)
    {
        // Damage cannot be negative
        if (value < 0)
            value = 0;

        Debug.Log(target.name + " takes " + value.ToString() + " damage");

        if (value > 0)
            target.StartCoroutine(target.Blink());

        // Apply element multipler
        if (Skill.elementsMultipliers.ContainsKey(target.elementAffinity) && Skill.elementsMultipliers[target.elementAffinity].ContainsKey(element))
        {
            float mult = Skill.elementsMultipliers[target.elementAffinity][element];
            Debug.Log("Dealing damage multiplied by: " + mult.ToString());
            value = Mathf.RoundToInt(value * mult);
        }

        // Subtract Hit Points
        target.hitPoints = target.hitPoints - value;

        // Jumping damage text
        DamageText dmgText = (Resources.Load("DamageText") as GameObject).GetComponent<DamageText>();
        Vector3 dmgPos = Camera.main.WorldToScreenPoint(target.transform.position);
        Instantiate(dmgText, dmgPos, Quaternion.identity, GameObject.Find("Canvas").transform).Setup(value.ToString(), lifeTime);

    }

    public static void Heal(Battler target, int value, float lifeTime = 0.5f)
    {
        Debug.Log("Restored " + value.ToString() + " HP to " + target.name);

        target.hitPoints += value;
        if (target.hitPoints >= target.maxHP.value)
            target.hitPoints = target.maxHP.value;

        // Jumping heal text
        DamageText healTxt = (Resources.Load("HealText") as GameObject).GetComponent<DamageText>();
        Vector3 dmgPos = Camera.main.WorldToScreenPoint(target.transform.position);
        Instantiate(healTxt, dmgPos, Quaternion.identity, GameObject.Find("Canvas").transform).Setup(value.ToString(), lifeTime);

        ////if (target.GetComponent<HeroBattler>() != null)
        //target.RefreshHUD();
    }

    public static void PlaySound(string clipName, bool loop = false, float volume = 1)
    {
        GameObject audio = new GameObject(clipName);
        AudioSource audioSource = audio.AddComponent<AudioSource>();
        audioSource.clip = Resources.Load("AudioClips/" + clipName, typeof(AudioClip)) as AudioClip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audio, audioSource.clip.length);
    }
    public static void PlaySound(AudioClip clip, bool loop = false, float volume = 1)
    {
        GameObject audio = new GameObject(clip.name);
        AudioSource audioSource = audio.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audio, audioSource.clip.length);
    }

    public static IEnumerator PlayAnimation(Battler actor, string animationName, bool wait)
    {
        if (animationName == "none") yield break; // Jump this part if battler is waiting

        if (actor.GetComponent<Animator>() == null)
        {
            Log("Trying to play an animation on a Battler without Animator Component", "Warning");
            yield break;
        }
        if (animationName == "")
        {
            Log(animationName + " animation not assigned!", "Warning");
            yield break;
        }

        Log("Playing animation " + animationName);

        if (actor.anim.parameters.Select(o => o.name).ToArray().Contains(animationName))
            actor.anim.SetTrigger(animationName);
        else
            if (Debug.isDebugBuild)
            Debug.LogWarning(actor.name + " does not have a " + animationName + " animation");
        
        if (wait) // Wait for the animation to finish
        {
            //yield return new WaitForSeconds(0.1f);
            yield return new WaitForEndOfFrame();

            float percentage = actor.GetEffectDelay(animationName);
            if (percentage != 0f)
                yield return new WaitForSeconds(actor.anim.GetCurrentAnimatorStateInfo(0).length * percentage);
            else
                yield return new WaitForSeconds(actor.anim.GetCurrentAnimatorStateInfo(0).length);
        }

        yield return null;
    }

    public static IEnumerator PlayEffect(Battler actor, Effect effect, bool wait=false)
    {
        if (actor == null)
            yield break;

        // Instantiate effect. Effect GameObject pos is added to target pos in order to manage effect offset.
        Effect e;
        if (effect != null)
        {
            e = Instantiate(effect, (actor.transform.position + (effect.transform.position * -(float)actor.faction)), Quaternion.identity, actor.transform);
            if (actor.faction == Battler.Faction.Heroes)
                e.GetComponent<SpriteRenderer>().flipX = true;

            // Wait for the effect to end
            if (wait)
                while (e != null)
                    yield return null;
        }
        else
            Debug.LogWarning("Effect not assigned!");

        yield return null;
    }
    public static IEnumerator PlayEffect(Vector3 position, Effect effect, Battler.Faction targetFaction, bool wait = false)
    {
        // Instantiate effect. Effect GameObject pos is added to target pos in order to manage effect offset.
        Effect e;
        if (effect != null)
        {
            e = Instantiate(effect, position, Quaternion.identity, GameObject.Find("Game Controller").GetComponent<GameController>().battleStuffInstance.transform);
            if (targetFaction == Battler.Faction.Heroes)
            {
                SpriteRenderer spr = e.GetComponent<SpriteRenderer>();
                spr.flipX = !spr.flipX;
            }

            // Wait for the effect to end
            if (wait)
                while (e != null)
                    yield return null;
        }
        else
            Debug.LogWarning("Effect not assigned!");
    }

    public static float GetClipLenght(Animator anim, string clipName)
    {
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;    //Get Animator controller
        for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
        {
            if (ac.animationClips[i].name == clipName)        //If it has the same name as your clip
            {
                return ac.animationClips[i].length;
            }
        }
        return 0;
    }
    public static IEnumerator JumpAway(GameObject go, Vector3 direction, float power = 200f)
    {
        if (go == null)
            yield break;

        if (go.GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            while (true)
            {
                if (go == null)
                    yield break;

                rb.AddForce(direction * power);
                yield return null;
                //yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            Debug.LogWarning(go.name + " has no rigidbody and cannot jump away!");
            yield break;
        }
    }

    public static void SetupHeroesSelection(HeroBattler[] availables, int selectables, HeroBattler[] alreadySelected = null, bool forceSelectMaximum = false, string title="")
    {
        GameController.Instance.currentSelectionMenu = Instantiate(Resources.Load("Menu/PartyMenu") as GameObject, GameController.Instance.mapCamera.transform).GetComponent<PartyMenu>();
        GameController.Instance.currentSelectionMenu.forceSelectMaximum = forceSelectMaximum;
        GameController.Instance.currentSelectionMenu.Setup();
        GameController.Instance.currentSelectionMenu.SelectionSetup(availables, selectables, alreadySelected, title);
    }

    public static IEnumerator HeroesSelection(HeroBattler[] availables, int selectables, Action callback, HeroBattler[] alreadySelected=null, bool forceSelectMaximum=false, string title="")
    {
        if (GameController.Instance.currentSelectionMenu != null)
            yield break;

        // Freeze player
        if (GameController.Instance.player != null)
            GameController.Instance.player.canMove = false;

        // Clear cache list for selected heroes
        GameController.Instance.selectionCache.Clear();

        // Create selection menu
        SetupHeroesSelection(availables, selectables, alreadySelected, forceSelectMaximum, title);        

        // Wait for player to select heroes
        while (GameController.Instance.currentSelectionMenu != null)
            yield return null;

        // Execute callback function after selection ended
        callback();

        // Unfreeze player
        if (GameController.Instance.player != null)
            GameController.Instance.player.canMove = true;
        yield return null;
    }

    // Callback to update party with selection cache
    public static void PartySelectionCallback()
    {
        GameController.Instance.partyPrefabs.Clear();
        foreach (HeroBattler b in GameController.Instance.selectionCache)
        {
            GameController.Instance.partyPrefabs.Add(b);
        }

        // Check if at least 3 else add random
        int count = GameController.Instance.partyPrefabs.Count;
        while (count < 3)
        {
            //foreach (HeroBattler h in GameController.instance.unlockedHeroes)
            HeroBattler h = GameController.Instance.unlockedHeroes[UnityEngine.Random.Range(0, GameController.Instance.unlockedHeroes.Length)];
            if (!GameController.Instance.partyPrefabs.Contains(h))
                GameController.Instance.partyPrefabs.Add(h);
            count = GameController.Instance.partyPrefabs.Count;
        }
    }

    public static void StartSelectionCallback()
    {
        // Check if at least 3 else add random
        int count = GameController.Instance.selectionCache.Count;
        while (count < 3)
        {
            Log("Selecting random starters");
            HeroBattler h = GameController.Instance.heroes[UnityEngine.Random.Range(0, GameController.Instance.heroes.Length - 1)];
            if (!GameController.Instance.selectionCache.Contains(h))
            {
                GameController.Instance.selectionCache.Add(h);
                Log("Automatically added " + h.name);
            }
            count = GameController.Instance.selectionCache.Count;
        }
        
        // Adding to party
        GameController.Instance.partyPrefabs.Clear();
        foreach (HeroBattler b in GameController.Instance.selectionCache)
        {
            GameController.Instance.partyPrefabs.Add(b);
        }

        // Only at start, set unlocked heroes to starting heroes
        GameController.Instance.unlockedHeroes = GameController.Instance.selectionCache.ToArray();

        Log("Done selecting starting characters");
    }

    public static void NewHeroCallback()
    {
        GameController.Instance.unlockedHeroes = GameController.Instance.unlockedHeroes.Concat(GameController.Instance.selectionCache).ToArray();
        Log("Done adding new hero");
    }

    [DllImport("__Internal")]
    private static extern bool IsMobile();
    public static bool IsMobileWebGL() { return IsMobile(); }

    //public static Battler FindParentBattler(Transform myTransform)
    //{
    //    if (myTransform.parent == null)
    //        return null;

    //    if (myTransform.parent.gameObject.GetComponent<Battler>() != null)
    //    {
    //        return myTransform.parent.gameObject.GetComponent<Battler>();
    //    }
    //    else
    //    {
    //        return FindParentBattler(myTransform.parent);
    //    }
    //}

}
