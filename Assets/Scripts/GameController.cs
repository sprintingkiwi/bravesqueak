using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Linq;

public class GameController : MonoBehaviour
{
    public enum Platform { Mobile, Other }
    [Header("Debug")]
    public Platform simulatedPlatform;    

    [Header("Saves")]
    public SaveSlot[] gameSlots;
    public bool isSaving;
    public bool justLoadedGameSlot;
    public bool canSave;
    public int actualSlot;
    public Dictionary<string, int> experience = new Dictionary<string, int>();
    public int partyLevel;
    public List<Perk> unlockedPerks = new List<Perk>();

    [Header("Battle")]
    public List<HeroBattler> partyPrefabs = new List<HeroBattler>();
    public List<EnemyBattler> enemiesPrefabs = new List<EnemyBattler>();
    //public Encounter encounter;
    public string currentEnemy;
    public string situation;
    public string lastBattleOutcome;

    [Header("Scenes and Maps")]
    public PlayerController player;
    public string lastScene;
    public WorldMap currentMap;
    public string savedCurrentMapName;
    public Vector3 playerStartPosition;
    //public List<string> defeatedNormalEnemies = new List<string>();
    public List<string> defeatedBosses = new List<string>();
    public bool inTransfer;
    public GameObject battleStuffPrefab;
    public GameObject areaStuffPrefab;
    public GameObject battleStuff;
    public GameObject areaStuff;
    public MapCameraController mapCamera;

    [Header("Music")]
    public AudioSource music;

    // Coroutines counters
    //public int fadingCoroutinesCount;
    //public int skillCoroutinesCount;

    // Save slot
    [System.Serializable]
    public class SaveSlot
    {
        public string name;
        public float gameTime;
        public Battler[] defeatedBosses;
    }

    // Persistence
    public static GameController cGInstance;
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

        // Mobile settings
        if (Jrpg.CheckPlatform() == "Mobile")
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }

    void Start()
    {
        music = gameObject.GetComponent<AudioSource>();

        // Populate experience dictionary
        experience.Add("Common", 0);
        experience.Add("Paladin", 0);
        experience.Add("Barbarian", 0);
        experience.Add("Dragoon", 0);
        experience.Add("Swordmaster", 0);
        experience.Add("Wizard", 0);
        experience.Add("Countermage", 0);
        experience.Add("Warlock", 0);
        experience.Add("Cleric", 0);
        experience.Add("Ranger", 0);
        experience.Add("Gunslinger", 0);
        experience.Add("Ninja", 0);


    }

    public void InitializeGame()
    {
        areaStuff = Instantiate(areaStuffPrefab);
        situation = "Map";
        mapCamera = areaStuff.transform.Find("Map Camera").GetComponent<MapCameraController>();

        GameObject intro = GameObject.Find("Intro");
        intro.GetComponent<Image>().enabled = true;

        // Instance current map
        WorldMap wm = (Resources.Load("Maps/" + savedCurrentMapName) as GameObject).GetComponent<WorldMap>();
        if (GameObject.Find(savedCurrentMapName) != null)
            Destroy(GameObject.Find(savedCurrentMapName));
        currentMap = Instantiate(wm, Vector3.zero, Quaternion.identity, areaStuff.transform);
        currentMap.name = savedCurrentMapName;
        currentMap.Setup();

        //// After-battle behaviour
        //if (situation.EndsWith("Battle"))
        //{
        //    // Here we can do something right after a battle ends
        //    if (lastBattleOutcome == "Win")
        //    {
        //        Debug.Log("Battle won, now destroying defeated enemy characters");
        //    }
        //    situation = "Map";
        //}      

        // Find player and set right player position
        player = areaStuff.transform.Find("Player").GetComponent<PlayerController>();
        player.transform.position = playerStartPosition;

        // If just ended a battle or loaded a new map-scene, save the game
        if (!justLoadedGameSlot) // But not right after a game-slot loading
            Save(actualSlot);
        else
            justLoadedGameSlot = false;

        Jrpg.Fade(GameObject.Find("Intro"), 0, 1);
    }

    public IEnumerator TriggerBattle(Encounter encounter, string enemyName)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Loading battle with " + encounter.name);

        player.canMove = false;

        Jrpg.PlaySound(currentMap.preBattleSound);

        // Visual and music fade out
        foreach (Coroutine c in new Coroutine[] { Jrpg.Fade(GameObject.Find("Intro"), 1), StartCoroutine(SetVolume(0)) })
            yield return c;

        //yield return new WaitForSeconds(2f);

        // Music Change
        if (encounter.battleMusic != null)
            music.clip = encounter.battleMusic;
        else
            music.clip = currentMap.defaultBattleMusic;
        music.Play();

        canSave = false;
        //playerStartPosition = GameObject.Find("Player").transform.position;
        currentEnemy = enemyName;

        //currentMap.gameObject.SetActive(false);
        areaStuff.SetActive(false);
        //Destroy(areaStuff);
        //Destroy(currentMap.gameObject);

        // Create battle stuff
        battleStuff = Instantiate(battleStuffPrefab);
        battleStuff.transform.Find("Battle Controller").GetComponent<BattleController>().Setup(encounter);
        situation = "Battle";

        // GUI Elements


        // Visual and music fade in
        foreach (Coroutine c in new Coroutine[] { Jrpg.Fade(GameObject.Find("Intro"), 0), StartCoroutine(SetVolume(1)) })
            yield return c;
    }

    public IEnumerator EndBattle(string outcome, Encounter encounter)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Ending battle");        

        if (outcome == "Win")
        {
            // Visual and music fade out
            foreach (Coroutine c in new Coroutine[] { Jrpg.Fade(GameObject.Find("Intro"), 1), StartCoroutine(SetVolume(0)) })
                yield return c;

            // Music Change
            music.clip = currentMap.soundtrack;
            music.Play();

            // Clear battle stuff
            Destroy(battleStuff);
            foreach (Transform t in GameObject.Find("BATTLE HUD").transform)
                Destroy(t.gameObject);

            // Destroy enemies
            if (currentEnemy != "Random")
            {
                Jrpg.Log("Destroying " + currentEnemy);
                Destroy(currentMap.transform.Find("ENEMIES").Find(currentEnemy).gameObject);
                if (encounter.boss)
                    defeatedBosses.Add(currentEnemy);
            }
            else
                Jrpg.Log("Not destroying enemies on map because it is a random encounter");

            areaStuff.SetActive(true);
            //currentMap.gameObject.SetActive(true);
            //areaStuff = Instantiate(areaStuffPrefab);
            //player.transform.position = playerStartPosition;

            situation = "Map";
            mapCamera.followPlayer = true;
            canSave = true;

            // GUI Elements


            // Visual and music fade in
            foreach (Coroutine c in new Coroutine[] { Jrpg.Fade(GameObject.Find("Intro"), 0), StartCoroutine(SetVolume(1)) })
                yield return c;

            player.canMove = true;
        }
        else // Defeat
        {
            yield return Jrpg.Fade(GameObject.Find("Intro"), 1);
            Jrpg.Log("You lose");
            yield return Jrpg.LoadScene("MainMenu");
        }        
    }

    public void Save(int slot)
    {

        isSaving = true;
        Jrpg.Log("SAVING IN SLOT " + actualSlot.ToString() + "...");

        Jrpg.SaveData saveData = new Jrpg.SaveData();
        saveData.savedCurrentMapName = currentMap.name;
        //saveData.lastScene = SceneManager.GetActiveScene().name;
        //Vector3 playerPos = GameObject.Find("Player").transform.position;
        //saveData.playerPosition = new float[] { playerPos.x, playerPos.y, playerPos.z };
        saveData.playerPosition = player.transform.position;

        // Save unlocked perks
        saveData.unlockedPerks.Clear();
        for (int s = 0; s < unlockedPerks.Count; s++)
        {
            saveData.unlockedPerks.Add(unlockedPerks[s].name);
        }

        // Save heroes data in an array
        for (int i = 0; i < saveData.heroesData.Length; i++)
        {
            saveData.heroesData[i].unlockedSkills.Clear();
            // Save each hero's unlocked skills
            for (int s = 0; s < partyPrefabs[i].unlockedSkills.Count; s++)
            {
                saveData.heroesData[i].unlockedSkills.Add(partyPrefabs[i].unlockedSkills[s].name);
            }

            // Save each hero's equipped skills
            for (int s = 0; s < partyPrefabs[i].skills.Length; s++)
            {
                if (partyPrefabs[i].skills[s] != null)
                    saveData.heroesData[i].equippedSkills[s] = partyPrefabs[i].skills[s].name;
                else
                    saveData.heroesData[i].equippedSkills[s] = "Empty";
            }

            // Save each hero's equipped perks
            for (int s = 0; s < partyPrefabs[i].perksPrefabs.Length; s++)
            {
                if (partyPrefabs[i].perksPrefabs[s] != null)
                    saveData.heroesData[i].equippedPerks[s] = partyPrefabs[i].perksPrefabs[s].name;
                else
                    saveData.heroesData[i].equippedPerks[s] = "Empty";
            }
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream saveFile = File.Open(Application.persistentDataPath + "/SaveData_SLOT_" + actualSlot.ToString() + ".dat", FileMode.OpenOrCreate);

        bf.Serialize(saveFile, saveData);
        saveFile.Close();

        isSaving = false;
    }

    public void Load(int slot)
    {
        Jrpg.Log("LOADING SLOT " + actualSlot.ToString() + "...");

        //if (File.Exists(Application.persistentDataPath + "/ SaveData_SLOT_" + ps.actualSlot.ToString() + ".dat"))
        //{            
        //}
        BinaryFormatter bf = new BinaryFormatter();
        FileStream saveFile = File.Open(Application.persistentDataPath + "/SaveData_SLOT_" + actualSlot.ToString() + ".dat", FileMode.Open);
        Jrpg.SaveData saveData = (Jrpg.SaveData)bf.Deserialize(saveFile);
        saveFile.Close();

        // Restore unlocked Perks
        unlockedPerks.Clear();
        for (int p = 0; p < saveData.unlockedPerks.Count; p++)
        {
            unlockedPerks.Add(Resources.Load("Skills/" + saveData.unlockedPerks[p], typeof(Perk)) as Perk);
        }

        // Restore Heroes data
        Debug.Log("Restoring heroes skills");
        for (int i = 0; i < partyPrefabs.Count; i++)
        {
            // Restore unlocked Skills
            partyPrefabs[i].unlockedSkills.Clear();
            for (int s = 0; s < saveData.heroesData[i].unlockedSkills.Count; s++)
            {
                partyPrefabs[i].unlockedSkills.Add(Resources.Load("Skills/" + saveData.heroesData[i].unlockedSkills[s], typeof(Skill)) as Skill);
            }

            // Restore equipped Skills
            for (int s = 0; s < saveData.heroesData[i].equippedSkills.Length; s++)
            {
                if (saveData.heroesData[i].equippedSkills[s] != "Empty")
                    partyPrefabs[i].skills[s] = Resources.Load("Skills/" + saveData.heroesData[i].equippedSkills[s], typeof(Skill)) as Skill;
                else
                    partyPrefabs[i].skills[s] = null;
            }

            // Restore equipped Perks
            for (int p = 0; p < saveData.heroesData[i].equippedPerks.Length; p++)
            {
                if (saveData.heroesData[i].equippedPerks[p] != "Empty")
                    partyPrefabs[i].perksPrefabs[p] = Resources.Load("Skills/" + saveData.heroesData[i].equippedPerks[p], typeof(Perk)) as Perk;
                else
                    partyPrefabs[i].perksPrefabs[p] = null;
            }
        }

        // Setting player start position
        playerStartPosition = saveData.playerPosition;
        
        // Map stuff
        savedCurrentMapName = saveData.savedCurrentMapName;
        justLoadedGameSlot = true;

        // Loading World scene
        StartCoroutine(Jrpg.LoadScene("World"));
    }

    public Coroutine Fade(GameObject target, float alpha, float speed, bool destroyAfter)
    {
        if (target.GetComponent<Image>() != null)
            return StartCoroutine(ProcessFadingImage(target, alpha, speed, destroyAfter));
        else if (target.GetComponent<Text>() != null)
            return StartCoroutine(ProcessFadingText(target, alpha, speed, destroyAfter));
        else
            return StartCoroutine(ProcessFading(target, alpha, speed, destroyAfter));
    }

    public IEnumerator ProcessFading(GameObject target, float alpha = 1, float speed = 1f, bool destroyAfter = false)
    {
        Coroutine[] children = new Coroutine[target.transform.childCount];

        // Recursion
        for(int i = 0; i < target.transform.childCount; i++)
            children[i] = StartCoroutine(ProcessFading(target.transform.GetChild(i).gameObject, alpha, speed, destroyAfter));

        if (target.GetComponent<SpriteRenderer>() != null)
        {
            string targetName = target.name;

            SpriteRenderer spr = target.GetComponent<SpriteRenderer>();
            float min = spr.color.a;

            Debug.Log("Fading " + target.name + " from " + min.ToString() + " to " + alpha.ToString());

            // Disable collider (if any) to avoid input while fading out
            if (target.GetComponent<Collider2D>() != null)
            {
                if (alpha == 0)
                    target.GetComponent<Collider2D>().enabled = false;
                else
                    target.GetComponent<Collider2D>().enabled = true;
            }

            float startTime = Time.time;
            float t = 0;
            while (t < 1)
            {
                t = (Time.time - startTime) / speed;
                if (spr != null)
                    spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Mathf.SmoothStep(min, alpha, t));
                else
                {
                    Jrpg.Log("TRYING TO FADE A NULL OBJECT: " + targetName, "Warning");
                    yield break;
                }
                yield return null;
            }
        }

        //fadingCoroutinesCount -= 1;
        foreach (Coroutine c in children)
            yield return c;

        if (destroyAfter)
            Destroy(target);
    }

    public IEnumerator ProcessFadingImage(GameObject target, float alpha = 1, float speed = 1f, bool destroyAfter = false)
    {
        // Recursion
        foreach (Transform child in target.transform)
            StartCoroutine(ProcessFadingImage(child.gameObject, alpha, speed, destroyAfter));

        if (target.GetComponent<Image>() != null)
        {
            Image spr = target.GetComponent<Image>();
            float min = spr.color.a;

            Debug.Log("Fading " + target.name + " from " + min.ToString() + " to " + alpha.ToString());

            // Disable collider (if any) to avoid input while fading out
            if (target.GetComponent<Collider2D>() != null)
            {
                if (alpha == 0)
                    target.GetComponent<Collider2D>().enabled = false;
                else
                    target.GetComponent<Collider2D>().enabled = true;
            }

            float startTime = Time.time;
            float t = 0;
            while (t < 1)
            {
                t = (Time.time - startTime) / speed;
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Mathf.SmoothStep(min, alpha, t));
                yield return null;
            }
        }

        if (destroyAfter)
        {
            while (target.transform.childCount > 0)
                yield return null;
            Destroy(target);
        }

        //fadingCoroutinesCount -= 1;
    }

    public IEnumerator ProcessFadingText(GameObject target, float alpha = 1, float speed = 1f, bool destroyAfter = false)
    {
        // Recursion
        foreach (Transform child in target.transform)
            StartCoroutine(ProcessFadingText(child.gameObject, alpha, speed, destroyAfter));

        if (target.GetComponent<Text>() != null)
        {
            Text spr = target.GetComponent<Text>();
            float min = spr.color.a;

            Debug.Log("Fading " + target.name + " from " + min.ToString() + " to " + alpha.ToString());

            // Disable collider (if any) to avoid input while fading out
            if (target.GetComponent<Collider2D>() != null)
            {
                if (alpha == 0)
                    target.GetComponent<Collider2D>().enabled = false;
                else
                    target.GetComponent<Collider2D>().enabled = true;
            }

            float startTime = Time.time;
            float t = 0;
            while (t < 1)
            {
                t = (Time.time - startTime) / speed;
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, Mathf.SmoothStep(min, alpha, t));
                yield return null;
            }
        }

        if (destroyAfter)
        {
            while (target.transform.childCount > 0)
                yield return null;
            Destroy(target);
        }

        //fadingCoroutinesCount -= 1;
    }

    public IEnumerator ChangeMusic(AudioClip newClip, float speed = 1f)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Music changing to " + newClip.name);

        float oldVolume = music.volume;

        yield return StartCoroutine(SetVolume(0));

        music.Stop();
        music.clip = newClip;
        music.Play();

        yield return StartCoroutine(SetVolume(oldVolume));
    }

    public IEnumerator SetVolume(float newVolume, float speed = 1f)
    {
        if (Debug.isDebugBuild)
            Debug.Log("Fading music volume to " + newVolume.ToString());

        float volume = music.volume;
        float startTime = Time.time;
        float t = 0;

        while (t < 1)
        {
            t = (Time.time - startTime) / speed;
            music.volume = Mathf.SmoothStep(volume, newVolume, t);
            yield return null;
        }
    }

    public IEnumerator ProcessTransfer(Collider2D collision, Transfer transfer, float speed = 1)
    {
        Debug.Log("Transfering player " + name);
        
        Debug.Log("Freezing player");
        player.canMove = false;

        Debug.Log("Check music change");
        if (transfer.destinationMap.soundtrack != music.clip)
            StartCoroutine(SetVolume(0, speed));

        Debug.Log("Fading out and pointing old map");
        // This will also wait for the music to fade out, if needed (the speed is the same)
        yield return Jrpg.Fade(GameObject.Find("Intro"), 1, speed);
        WorldMap destinationMap = transfer.destinationMap;
        GameObject oldMap = currentMap.gameObject;
        yield return null;

        Debug.Log("Instance new map");
        inTransfer = true;
        transfer.transfering = true;
        currentMap = Instantiate(destinationMap, Vector3.zero, Quaternion.identity, areaStuff.transform);
        currentMap.name = transfer.destinationMap.name;
        currentMap.Setup();

        Debug.Log("Find destination place");
        Transfer instDestPlace = currentMap.transform.Find("TRANSFERS").transform.Find(transfer.destinationPlace).GetComponent<Transfer>();
        instDestPlace.transfering = true;

        Debug.Log("Reset camera");
        MapCameraController mainCamera = GameObject.Find("Map Camera").GetComponent<MapCameraController>();
        mainCamera.Setup();
        //defeatedNormalEnemies.Clear();

        Debug.Log("Move player");
        player.transform.position = instDestPlace.transform.position;
        player.lastCheckedPos4RandEncounters = player.transform.position;

        Debug.Log("Destroying old map");
        Destroy(oldMap);

        Debug.Log("Fading in");
        yield return Jrpg.Fade(GameObject.Find("Intro"), 0, 1);

        Debug.Log("Unfreezing player");
        player.canMove = true;
    }
}
