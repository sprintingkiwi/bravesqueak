﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
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
        public Vector2 split;
    }
    [Header("Map Layers")]
    public MapLayer[] mapLayers;    

    [Header("System")]
    public SpriteRenderer cameraBoundary;

    private void Start()
    {
        GameController.Instance.InitializeMap(this);
    }

    public virtual void BuildMap()
    {
        foreach (MapLayer ml in mapLayers)
            DisposeMapTiles(ml.folderName, ml.sortingOrder, ml.zLevel, ml.split);
    }

    // Use this for initialization
    public virtual void Setup()
    {      
        // DO SOME STUFF RIGHT AFTER THE MAP LOADS:        
        // Init music
        //if (soundtrack != GameController.Instance.music.clip)
        //{
        //    GameController.Instance.music.Stop();
        //    GameController.Instance.music.clip = soundtrack;
        //    GameController.Instance.music.Play();
        //    StartCoroutine(GameController.Instance.SetVolume(1));
        //}

        // Enemies names
        foreach (Transform t in transform.Find("ENEMIES"))
        {
            t.name += "_ID" + t.GetSiblingIndex().ToString() + "_" + name;
        }

        // Destroy defeated enemies
        foreach (string de in GameController.Instance.defeatedBosses.ToArray())
        {
            Debug.Log("Destroying " + de);
            Destroy(GameObject.Find(de));
        }

        // A Star Pathfinding
        //if (GameObject.Find("A_Star") != null)
        //{
        //    AstarPath astar = GameObject.Find("A_Star").GetComponent<AstarPath>();
        //    astar.Scan();
        //}
    }

    // Function to build map on the editor
    public void DisposeMapTiles(string layerName, int sortingOrder, float zLevel, Vector2 mapSplit)
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
        for (int column = 0; column < mapSplit.x; column++)
        {
            for (int row = 0; row < mapSplit.y; row++)
            {                  
                // Create tile
                GameObject tile = new GameObject("tile_" + tileCount.ToString());
                tile.transform.parent = container;
                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                string tileDir = name;
                sr.sprite = Resources.Load<Sprite>("Maps/" + tileDir + "/" + layerName.ToUpper() + "/" + tileDir + "_" + layerName +"_" + tileCount.ToString());
                sr.sortingOrder = sortingOrder;

                // Find right position (Rounding to 2 decimal places because with 100 pixel per unit it is pixel perfect)
                Vector3 pos = new Vector3(
                    Mathf.Round((startPos.x + (boundary.bounds.size.x / mapSplit.x) * column) * 100) / 100,
                    Mathf.Round((startPos.y - (boundary.bounds.size.y / mapSplit.y) * row) * 100) / 100,
                    zLevel);
                tile.transform.position = pos;

                tileCount += 1;
            }
        }
    }

    public virtual Encounter ChooseRandomEncounter()
    {
        Jrpg.Log("Weighted random selection of a random encounter");

        // Calculate sum of weigths
        int sumOfWeights = 0;
        foreach (RandomEncounter re in randomEncounters)
        {
            if (re == null)
                continue;

            sumOfWeights += re.weight;
        }

        // Take random number greater than 0 and less than sum of weights
        int rnd = Random.Range(0, sumOfWeights);

        // Log
        foreach (RandomEncounter re in randomEncounters)
        {
            if (re == null)
                continue;
            Jrpg.Log(re.encounter.name);
        }            

        // Algorithm
        foreach (RandomEncounter re in randomEncounters)
        {
            if (re == null)
                continue;

            if (rnd < re.weight)
                return re.encounter;
            rnd -= re.weight;
        }
        Debug.Log("Should never execute this");
        return null;
    }
}
