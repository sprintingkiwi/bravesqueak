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
    public bool showDebugInfo;

    [Header("Saves")]
    //public SaveSlot[] gameSlots;
    public Jrpg.SaveData[] saveData = new Jrpg.SaveData[4];
    public bool isSaving;
    public bool isLoading;
    public bool justLoadedGameSlot;
    public bool canSaveLoad;
    public int currentSaveSlot = 0;
    public bool continueGame;
    public Dictionary<string, int> experience = new Dictionary<string, int>();
    public int partyLevel;
    //Inventory
    public List<Skill> unlockedSkills = new List<Skill>();
    public List<Perk> unlockedPerks = new List<Perk>();
    public List<Food> foods = new List<Food>();

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
    public HeroBattler[] heroes;
    public bool unlockAll;
    public HeroBattler[] unlockedHeroes;
    public PartyMenu currentSelectionMenu;

    // This is to be used as a cache list for heroes selection menu in various situations
    public List<HeroBattler> selectionCache = new List<HeroBattler>();

    public HeroMenu heroMenu;
    public ItemSelectionMenu itemSelectionMenu;

    [Header("Music")]
    public AudioSource music;

    [Header("UI")]
    public Font activeFont;
    public Font defaultFont;
    public Font accessibleFont;

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
    public static GameController Instance;
    void Awake()
    {
        // Persistence
        if (Instance == null)
        {
            Instance = this;
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

    public void StartNewGame()
    {
        //// Choose random starter battlers
        //Jrpg.Log("Choosing random starters");
        //partyPrefabs = new List<HeroBattler>();
        //for (int i = 0; i < 3; i++)
        //{
        //    while (partyPrefabs.Count < 3)
        //    {
        //        HeroBattler b = heroes[UnityEngine.Random.Range(0, 7)];
        //        if (!partyPrefabs.Contains(b))
        //        {
        //            partyPrefabs.Add(b);
        //            Jrpg.Log("Added " + b.name);
        //        }
        //    }
        //}

        // Start Everything
        StartCoroutine(Jrpg.JumpAway(GameObject.Find("Title"), Vector3.up));
        StartCoroutine(Jrpg.JumpAway(GameObject.Find("Play"), Vector3.down, power: 20f));
        StartCoroutine(Jrpg.JumpAway(GameObject.Find("New Game"), Vector3.down, power: 30f));
        StartCoroutine(Jrpg.LoadScene("World"));
    }

    public void FirstHeroesSelection()
    {
        // Starting heroes selection
        partyPrefabs = new List<HeroBattler>();
        unlockedHeroes = partyPrefabs.ToArray();
        StartCoroutine(Jrpg.HeroesSelection(heroes, 3, Jrpg.StartSelectionCallback, forceSelectMaximum: true, title: "CHOOSE YUOR HEROES"));
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

        Jrpg.Fade(GameObject.Find("Intro"), 0, 1);
        

        if (continueGame) // CONTINUE
        {
            canSaveLoad = true;
            StartCoroutine(Load(currentSaveSlot));
        }
        else // NEW GAME
        {
            // First Hero Selection
            if (!unlockAll)
            {
                FirstHeroesSelection();
            }
            else
            {
                Jrpg.Log("Unlock all mode: unlocking all characters");
            }
            canSaveLoad = true;
        }
    }

    public IEnumerator TriggerBattle(Encounter encounter, string enemyName)
    {
        if (isSaving || isLoading)
        {
            Jrpg.Log("Cannot trigger battle when saving or loading");
            yield break;
        }

        if (encounter == null)
            yield break;

        canSaveLoad = false;
        situation = "Battle";

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

        canSaveLoad = false;
        //playerStartPosition = GameObject.Find("Player").transform.position;
        currentEnemy = enemyName;

        //currentMap.gameObject.SetActive(false);
        areaStuff.SetActive(false);
        //Destroy(areaStuff);
        //Destroy(currentMap.gameObject);

        // Create battle stuff
        battleStuff = Instantiate(battleStuffPrefab);
        battleStuff.transform.Find("Battle Controller").GetComponent<BattleController>().Setup(encounter);

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

            // Battle Tips
            if (encounter.type != Encounter.Type.Boss)
                yield return StartCoroutine(ShowBattleTip(-1));
            else
                Jrpg.Log("Congrats! You defeated a Boss!");

            // Music Change
            music.clip = currentMap.soundtrack;
            music.Play();

            // Clear battle stuff
            Destroy(battleStuff);
            // Destroy HUD
            foreach (Transform t in GameObject.Find("BATTLE HUD").transform)
                Destroy(t.gameObject);

            // Destroy enemies
            if (currentEnemy != "Random")
            {
                Jrpg.Log("Destroying " + currentEnemy);
                GameObject enemyToDestroy = currentMap.transform.Find("ENEMIES").Find(currentEnemy).gameObject;
                if (enemyToDestroy != null)
                    Destroy(enemyToDestroy);
                if (encounter.type != Encounter.Type.Common)
                    defeatedBosses.Add(currentEnemy);
            }
            else
                Jrpg.Log("Not destroying enemies on map because it is a random encounter");

            areaStuff.SetActive(true);
            //currentMap.gameObject.SetActive(true);
            //areaStuff = Instantiate(areaStuffPrefab);
            //player.transform.position = playerStartPosition;

            // GUI Elements
            

            // Visual and music fade in
            foreach (Coroutine c in new Coroutine[] { Jrpg.Fade(GameObject.Find("Intro"), 0), StartCoroutine(SetVolume(1)) })
                yield return c;

            situation = "Map";

            // Select new hero if defeated boss
            if (encounter.type == Encounter.Type.Boss)
            {
                List<HeroBattler> remainingHeroes = new List<HeroBattler>();
                foreach (HeroBattler hb in heroes)
                    if (!unlockedHeroes.Contains(hb))
                        remainingHeroes.Add(hb);

                if (remainingHeroes.Count > 0)
                {
                    Jrpg.Log("Starting new hero selection");
                    yield return StartCoroutine(Jrpg.HeroesSelection(remainingHeroes.ToArray(), 1, Jrpg.NewHeroCallback, forceSelectMaximum: true, title: "CHOOSE YUOR HEROES"));
                }
            }

            // Restore normal map situation
            canSaveLoad = true;
            mapCamera.followPlayer = true;
            player.canMove = true;

            // Save at the end of each battle at slot 0 (?)
            yield return StartCoroutine(Save(currentSaveSlot));
        }
        else // Defeat
        {
            yield return Jrpg.Fade(GameObject.Find("Intro"), 1);
            Jrpg.Log("You lose", "Build");

            // Load main menu when defeated (?)
            //yield return StartCoroutine(Jrpg.LoadScene("MainMenu"));

            // Load slot 0 when defeated (?)
            music.clip = currentMap.soundtrack;
            music.Play();
            Destroy(battleStuff);
            foreach (Transform t in GameObject.Find("BATTLE HUD").transform)
                Destroy(t.gameObject);
            areaStuff.SetActive(true);
            situation = "Map";
            canSaveLoad = true;
            mapCamera.followPlayer = true;
            player.canMove = true;
            yield return StartCoroutine(Load(currentSaveSlot));
        }        
    }

    public IEnumerator ShowBattleTip(int tipID = 0)
    {
        Jrpg.Log("Defeated common enemy or miniboss, now showing battle tip");
        UnityEngine.Object[] tips = Resources.LoadAll("BattleTips", typeof(GameObject));
        GameObject tip;
        if (tipID == -1)
            tip = Instantiate(tips[UnityEngine.Random.Range(0, tips.Length)] as GameObject, battleStuff.transform.Find("Battle Camera"));
        else
            tip = Instantiate(tips[tipID] as GameObject, battleStuff.transform.Find("Battle Camera"));
        yield return Jrpg.Fade(GameObject.Find("Intro"), 0, speed: 0.2f);
        while (!Input.anyKeyDown && !(Input.touchCount > 0))
            yield return null;
        yield return Jrpg.Fade(GameObject.Find("Intro"), 1, speed: 0.2f);
        Destroy(tip);
    }

    public IEnumerator Save(int slot)
    {
        if (!canSaveLoad || isSaving || isLoading)
        {
            Jrpg.Log("Cannot save now");
            yield break;
        }

        isSaving = true;
        player.canMove = false;
        Jrpg.Log("SAVING IN SLOT " + (slot + 1).ToString() + " ...", "Build", 2f);

        //saveData = new Jrpg.SaveData();
        saveData[slot].savedCurrentMapName = currentMap.name;
        //saveData.lastScene = SceneManager.GetActiveScene().name;
        Vector3 playerPos = player.transform.position;
        saveData[slot].playerPosition = new float[] { playerPos.x, playerPos.y, playerPos.z };
        saveData[slot].defeatedBosses = defeatedBosses.ToArray();

        // Save unlocked heroes
        saveData[slot].unlockedHeroes.Clear();
        foreach (HeroBattler hb in unlockedHeroes)
            saveData[slot].unlockedHeroes.Add(hb.name);

        // Save party heroes
        saveData[slot].partyHeroes.Clear();
        foreach (HeroBattler hb in partyPrefabs.ToArray())
            saveData[slot].partyHeroes.Add(hb.name);

        //// Save unlocked skills
        //saveData[slot].unlockedSkills.Clear();
        //for (int s = 0; s < unlockedSkills.Count; s++)
        //{
        //    if (unlockedSkills[s] != null)
        //        saveData[slot].unlockedSkills.Add(unlockedSkills[s].name);
        //}

        //// Save unlocked perks
        //saveData[slot].unlockedPerks.Clear();
        //for (int s = 0; s < unlockedPerks.Count; s++)
        //{
        //    if (unlockedPerks[s] != null)
        //        saveData[slot].unlockedPerks.Add(unlockedPerks[s].name);
        //}

        //// Save heroes data in an array
        //for (int i = 0; i < saveData[slot].heroesData.Length; i++)
        //{
        //    Jrpg.Log("Saving data for hero " + heroes[i].name);
        //    saveData[slot].heroesData[i].name = heroes[i].name;

        //    // Save each hero's equipped skills
        //    for (int s = 0; s < heroes[i].skills.Length; s++)
        //    {
        //        if (heroes[i].skills[s] != null)
        //            saveData[slot].heroesData[i].equippedSkills[s] = heroes[i].skills[s].name;
        //        else
        //            saveData[slot].heroesData[i].equippedSkills[s] = "Empty";
        //    }

        //    // Save each hero's equipped perks
        //    for (int s = 0; s < heroes[i].perks.Length; s++)
        //    {
        //        if (heroes[i].perks[s] != null)
        //            saveData[slot].heroesData[i].equippedPerks[s] = heroes[i].perks[s].name;
        //        else
        //            saveData[slot].heroesData[i].equippedPerks[s] = "Empty";
        //    }
        //}

        BinaryFormatter bf = new BinaryFormatter();
        FileStream saveFile = File.Open(Application.persistentDataPath + "/SaveData_SLOT_" + slot.ToString() + ".dat", FileMode.OpenOrCreate);

        bf.Serialize(saveFile, saveData[slot]);
        saveFile.Close();

        player.canMove = true;
        isSaving = false;

        yield return null;
    }

    public IEnumerator Load(int slot)
    {
        if (!canSaveLoad || isSaving || isLoading)
        {
            Jrpg.Log("Cannot load now", "Warning");
            yield break;
        }

        if (!File.Exists(Application.persistentDataPath + "/SaveData_SLOT_" + slot.ToString() + ".dat"))
        {
            Jrpg.Log("Missing save file", "Build");
            Jrpg.Log("Missing save file", "Warning");
            yield break;
        }
        else
            Jrpg.Log("Save file found");

        isLoading = true;
        player.canMove = false;

        Jrpg.Log("LOADING SLOT " + slot.ToString() + "...");
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream saveFile = File.Open(Application.persistentDataPath + "/SaveData_SLOT_" + slot.ToString() + ".dat", FileMode.Open);
        Jrpg.SaveData saveData = (Jrpg.SaveData)bf.Deserialize(saveFile);
        saveFile.Close();

        //// Restore unlocked Skills
        //unlockedSkills.Clear();
        //for (int p = 0; p < saveData.unlockedSkills.Count; p++)
        //{
        //    unlockedSkills.Add(Resources.Load("Skills/" + saveData.unlockedSkills[p], typeof(Skill)) as Skill);
        //}

        //// Restore unlocked Perks
        //unlockedPerks.Clear();
        //for (int p = 0; p < saveData.unlockedPerks.Count; p++)
        //{
        //    unlockedPerks.Add(Resources.Load("Perks/" + saveData.unlockedPerks[p], typeof(Perk)) as Perk);
        //}

        //// Restore Heroes data
        //Debug.Log("Restoring heroes skills");
        //for (int i = 0; i < heroes.Count(); i++)
        //{
        //    // Restore equipped Skills
        //    for (int s = 0; s < saveData.heroesData[i].equippedSkills.Length; s++)
        //    {
        //        if (saveData.heroesData[i].equippedSkills[s] != "Empty")
        //            heroes[i].skills[s] = Resources.Load("Skills/" + saveData.heroesData[i].equippedSkills[s], typeof(Skill)) as Skill;
        //        else
        //            heroes[i].skills[s] = null;
        //    }

        //    // Restore equipped Perks
        //    for (int p = 0; p < saveData.heroesData[i].equippedPerks.Length; p++)
        //    {
        //        if (saveData.heroesData[i].equippedPerks[p] != "Empty")
        //            heroes[i].perks[p] = Resources.Load("Perks/" + saveData.heroesData[i].equippedPerks[p], typeof(Perk)) as Perk;
        //        else
        //            heroes[i].perks[p] = null;
        //    }
        //}

        // LOADING
        //yield return StartCoroutine(Jrpg.LoadScene("World"));

        // Unlocked heroes
        List<HeroBattler> tempUnlocked = new List<HeroBattler>();
        foreach(HeroBattler hb in heroes)
            if (saveData.unlockedHeroes.ToArray().Contains(hb.name))
                tempUnlocked.Add(hb);
        unlockedHeroes = tempUnlocked.ToArray();

        // Party heroes
        List<HeroBattler> tempParty = new List<HeroBattler>();
        foreach (HeroBattler hb in heroes)
            if (saveData.partyHeroes.ToArray().Contains(hb.name))
                tempParty.Add(hb);
        partyPrefabs = tempParty;

        // Defeated enemies (bosses)
        foreach (string db in saveData.defeatedBosses)
            if (!defeatedBosses.Contains(db))
                defeatedBosses.Add(db);
        foreach (string db in defeatedBosses.ToArray())
            if (!saveData.defeatedBosses.Contains(db))
                defeatedBosses.Remove(db);

        // Transfer to map
        Transfer t = new GameObject().AddComponent<Transfer>();
        t.name = "Loading Transfer";
        t.destinationMap = (Resources.Load("Maps/" + saveData.savedCurrentMapName) as GameObject).GetComponent<WorldMap>();
        t.destinationPos = new Vector3(saveData.playerPosition[0], saveData.playerPosition[1], saveData.playerPosition[2]);
        yield return StartCoroutine(ProcessTransfer(null, t));
        Destroy(t.gameObject);
        inTransfer = false;

        // Unlock player
        //justLoadedGameSlot = true;
        player.canMove = true;
        isLoading = false;

        Jrpg.Log("LOADED SLOT " + (slot + 1).ToString() + " ...", "Build", 2f);

        yield return null;
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
        if (transfer.destinationMap.soundtrack != null)
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
        if (transfer.destinationPlace != null)
        {
            Transfer instDestPlace = currentMap.transform.Find("TRANSFERS").transform.Find(transfer.destinationPlace).GetComponent<Transfer>();
            instDestPlace.transfering = true;
            Debug.Log("Move player");
            player.transform.position = instDestPlace.transform.position;
        }
        else
        {
            Debug.Log("Move player");
            player.transform.position = transfer.destinationPos;
        }
        player.lastCheckedPos4RandEncounters = player.transform.position;

        Debug.Log("Reset camera");
        MapCameraController mainCamera = GameObject.Find("Map Camera").GetComponent<MapCameraController>();
        mainCamera.Setup();
        //defeatedNormalEnemies.Clear();

        Debug.Log("Destroying old map");
        Destroy(oldMap);

        Debug.Log("Fading in");
        yield return Jrpg.Fade(GameObject.Find("Intro"), 0, 1);

        Debug.Log("Unfreezing player");
        player.canMove = true;
    }

    public void SetFont(bool accessible)
    {
        if (accessible)
            activeFont = accessibleFont;
        else
            activeFont = defaultFont;
    }

    public void SetSaveSlot(int slotID)
    {
        currentSaveSlot = slotID;
    }

    public void ContinueGame()
    {
        continueGame = true;
    }

}
