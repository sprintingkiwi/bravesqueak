using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransfer : MonoBehaviour
{
    GameController ps;
    public string destinationScene;
    public WorldMap destinationMap;
    public Vector3 destinationPosition;

	// Use this for initialization
	void Start ()
    {
        ps = GameObject.Find("Game Controller").GetComponent<GameController>();
        
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!ps.inTransfer)
        {
            ps.inTransfer = true;
            ps.lastScene = SceneManager.GetActiveScene().name;
            ps.playerStartPosition = destinationPosition;
            ps.savedCurrentMapName = destinationMap.name;
            //ps.defeatedNormalEnemies.Clear();
            if (destinationMap.soundtrack != ps.music.clip)
                StartCoroutine(Jrpg.LoadScene(destinationScene, new Coroutine[] { StartCoroutine(ps.SetVolume(0)) }));
            else
                StartCoroutine(Jrpg.LoadScene(destinationScene));
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (ps.inTransfer && SceneManager.GetActiveScene().name != ps.lastScene)
        {
            ps.inTransfer = false;
        }
    }
}
