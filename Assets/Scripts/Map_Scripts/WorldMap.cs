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
    public GameObject[] encounters;
    public SpriteRenderer cameraBoundary;

    [System.Serializable]
    public class MapLayer
    {
        public string folderName;
        public int sortingOrder;
        public float zLevel;
    }
    public MapLayer[] mapLayers;

    void Awake()
    {
        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    // Use this for initialization
    public void Setup()
    {
        foreach (MapLayer ml in mapLayers)
            DisposeMapTiles(ml.folderName, ml.sortingOrder, ml.zLevel);

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

        AstarPath astar = GameObject.Find("A_Star").GetComponent<AstarPath>();
        astar.Scan();
    }

    public void DisposeMapTiles(string layerName, int sortingOrder, float zLevel)
    {
        // Get container object
        Transform container = transform.Find(layerName.ToUpper());
        container.transform.position = new Vector3(container.transform.position.x, container.transform.position.y, zLevel);

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
}
