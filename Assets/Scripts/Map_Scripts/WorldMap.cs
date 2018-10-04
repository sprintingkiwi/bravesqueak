using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldMap : MonoBehaviour
{
    GameController gc;
    public AudioClip soundtrack;
    public AudioClip defaultBattleMusic;
    public AudioClip preBattleSound;
    public GameObject defaultBattleback;
    [Range(1, 100)]
    public int randomEncountersRate;

    [System.Serializable]
    public class RandomEncounter
    {
        [Range(1, 10)]
        public int weight;
        public Encounter encounter;
    }
    public RandomEncounter[] randomEncounters;    

    [System.Serializable]
    public class MapLayer
    {
        public string folderName;
        public int sortingOrder;
        public float zLevel;

        [Header("Animation")]
        public bool isAnimated;
        public float pause;
        // Folder name of the other frames
        public string[] nextFrames;
    }
    [Header("Map Layers")]
    public MapLayer[] mapLayers;    

    [Header("System")]
    public SpriteRenderer cameraBoundary;

    void Awake()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    void Update()
    {
        //if (transform.Find("Passability").GetComponent<PolygonCollider2D>().enabled == false)
        //{
        //    Debug.LogWarning(name + " map passability collider is not enabled. Enabling it now...");
        //    transform.Find("Passability").GetComponent<PolygonCollider2D>().enabled = true;
        //}
    }

    public virtual void BuildMap()
    {
        foreach (MapLayer ml in mapLayers)
            DisposeMapTiles(ml.folderName, ml.sortingOrder, ml.zLevel);
    }

    // Use this for initialization
    public virtual void Setup()
    {      
        // DO SOME STUFF RIGHT AFTER THE MAP LOADS:        
        // Init music
        if (soundtrack != gc.music.clip)
        {
            gc.music.Stop();
            gc.music.clip = soundtrack;
            gc.music.Play();
            StartCoroutine(gc.SetVolume(1));
        }

        // Enemies names
        foreach (Transform t in transform.Find("ENEMIES"))
        {
            t.name += "_ID" + t.GetSiblingIndex().ToString() + "_" + name;
        }

        // Destroy defeated enemies
        foreach (string de in gc.defeatedBosses.ToArray())
        {
            Debug.Log("Destroying " + de);
            Destroy(GameObject.Find(de));
        }

        // A Star Pathfinding
        if (GameObject.Find("A_Star") != null)
        {
            AstarPath astar = GameObject.Find("A_Star").GetComponent<AstarPath>();
            astar.Scan();
        }

        StartCoroutine(AnimateLayers());
    }

    // Function to build map on the editor
    public void DisposeMapTiles(string layerName, int sortingOrder, float zLevel)
    {
        // Get container object
        Transform container = transform.Find(layerName.ToUpper());
        container.transform.position = new Vector3(container.transform.position.x, container.transform.position.y, zLevel);

        // Delete old stuff
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in container)
            children.Add(child.gameObject);
        foreach (GameObject c in children)
            DestroyImmediate(c);

        // Finding start position
        SpriteRenderer boundary = container.GetComponent<SpriteRenderer>();
        boundary.sortingOrder = sortingOrder - 1;
        //boundary.color = Color.clear;
        Vector3 startPos = new Vector3(boundary.bounds.center.x - boundary.bounds.size.x / 2, boundary.bounds.center.y + boundary.bounds.size.y / 2, zLevel);

        int tileCount = 1;
        for (int column = 0; column < 6; column++)
        {
            for (int row = 0; row < 6; row++)
            {                  
                // Create tile
                GameObject tile = new GameObject("tile_" + tileCount.ToString());
                tile.transform.parent = container;
                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                string tileDir = name;
                sr.sprite = Resources.Load<Sprite>("Maps/" + tileDir + "/" + layerName.ToUpper() + "/" + tileDir + "_" + layerName +"_" + tileCount.ToString());
                sr.sortingOrder = sortingOrder;

                // Find right position
                Vector3 pos = new Vector3(startPos.x + (boundary.bounds.size.x / 6) * column, startPos.y - (boundary.bounds.size.y / 6) * row, zLevel);
                tile.transform.position = pos;

                tileCount += 1;
            }
        }
    }

    // Map Layers Animation Coroutine
    public IEnumerator AnimateLayers()
    {
        foreach(MapLayer mainLayer in mapLayers)
        {
            

            if (mainLayer.isAnimated)
            {
                List<List<Sprite>> spriteLists = new List<List<Sprite>>();
                spriteLists.Add(new List<Sprite>());

                List<string> containersNames = new List<string>();
                containersNames.Add(mainLayer.folderName);
                containersNames.Concat(mainLayer.nextFrames);

                foreach (string containerName in containersNames)
                {
                    List<Sprite> frameTiles = new List<Sprite>();
                    Transform container = transform.Find(containerName);
                    foreach (Transform tile in container)
                    {
                        frameTiles.Add(tile.GetComponent<SpriteRenderer>().sprite);
                    }
                }
            }
        }

        yield return null;
    }

    public virtual Encounter ChooseRandomEncounter()
    {
        Jrpg.Log("Weighted random selection of a random encounter");

        // Calculate sum of weigths
        int sumOfWeights = 0;
        foreach (RandomEncounter re in randomEncounters)
            sumOfWeights += re.weight;

        // Take random number greater than 0 and less than sum of weights
        int rnd = Random.Range(0, sumOfWeights);

        // Log
        foreach (RandomEncounter re in randomEncounters)
            Jrpg.Log(re.encounter.name);

        // Algorithm
        foreach (RandomEncounter re in randomEncounters)
        {
            if (rnd < re.weight)
                return re.encounter;
            rnd -= re.weight;
        }
        Debug.Log("Should never execute this");
        return null;
    }
}
