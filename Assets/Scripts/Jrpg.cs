using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class Jrpg : MonoBehaviour
{
    [System.Serializable]
    public class SaveData
    {
        public EnemyBattler[] defeatedBosses;
        public string savedCurrentMapName;
        //public string lastScene;
        public Vector3 playerPosition;
        public List<string> unlockedPerks = new List<string>();
        //public List<string>[] skillLists = new List<string>[] { new List<string>(), new List<string>(), new List<string>() };
        public HeroData[] heroesData = new HeroData[3];
    }

    [System.Serializable]
    public class HeroData
    {
        public List<string> unlockedSkills = new List<string>();
        public string[] equippedSkills;
        public string[] equippedPerks;
    }

    public static void Log(string text, string type = "")
    {
        if (Debug.isDebugBuild)
        {
            if (type == "Warning")
                Debug.LogWarning(text);
            else
                Debug.Log(text);

            if (type == "Visible")
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
                Destroy(log, 10f);
            }
        }        
    }

    public static int RollDice(int dice, int faces)
    {
        int result = 0;

        for (int i = 0; i < dice; i++)
        {
            result = result + Random.Range(1, faces);
        }

        return result;
    }

    public static int Roll(Stat stat, Roll roll = null, int modifier = 100)
    {
        int gap = 5;
        int result = 0;

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
        GameController gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        //ps.fadingCoroutinesCount += 1;
        return gc.Fade(target, alpha, speed, destroyAfter);
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

        GameController gc = GameObject.Find("Game Controller").GetComponent<GameController>();
                
        // Wait for the game to finish saving, just in case...
        while (gc.isSaving)
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
        yield return gc.StartCoroutine(gc.SetVolume(0));

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

        // Update HUD
        //if (target.GetComponent<HeroBattler>() != null)
        target.RefreshHUD();
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

        //if (target.GetComponent<HeroBattler>() != null)
        target.RefreshHUD();
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

    public static IEnumerator PlayAnimation(Battler actor, string name, bool wait)
    {
        Debug.Log("Playing animation " + name);
        if (name != "")
        {
            if (actor.anim.parameters.Select(o => o.name).ToArray().Contains(name))
                actor.anim.SetTrigger(name);
            else
                if (Debug.isDebugBuild)
                Debug.LogWarning(actor.name + " does not have a " + name + " animation");
        }
        else
            Debug.LogError(name + " animation not assigned!");

        if (wait) // Wait for the animation to finish
        {
            //yield return new WaitForSeconds(0.1f);
            yield return new WaitForEndOfFrame();

            float percentage = actor.GetEffectDelay(name);
            if (percentage != 0f)
                yield return new WaitForSeconds(actor.anim.GetCurrentAnimatorStateInfo(0).length * percentage);
            else
                yield return new WaitForSeconds(actor.anim.GetCurrentAnimatorStateInfo(0).length);
        }

        yield return null;
    }

    public static void PlayEffect(Battler actor, Effect effect)
    {
        // Instantiate effect. Effect GameObject pos is added to target pos in order to manage effect offset.
        Effect e;
        if (effect != null)
        {
            e = Instantiate(effect, (actor.transform.position + (effect.transform.position * -(float)actor.faction)), Quaternion.identity);
            if (actor.faction == Battler.Faction.Heroes)
                e.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            Debug.LogError(effect.name + " effect not assigned!");
    }
    public static void PlayEffect(Vector3 position, Effect effect, Battler.Faction targetFaction)
    {
        // Instantiate effect. Effect GameObject pos is added to target pos in order to manage effect offset.
        Effect e;
        if (effect != null)
        {
            e = Instantiate(effect, position, Quaternion.identity, GameObject.Find("BATTLE STUFF").transform);
            if (targetFaction == Battler.Faction.Heroes)
                e.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            Debug.LogError(effect.name + " effect not assigned!");
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
        if (go.GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            while (true)
            {
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

}
