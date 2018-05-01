using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : BattleCameraController
{
    public float rightBound;
    public float leftBound;
    public float topBound;
    public float bottomBound;
    public Vector3 pos;
    public Transform target;
    public SpriteRenderer spriteBounds;
    public GameController gc;
    public bool followPlayer;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        gc = GameObject.Find("Game Controller").GetComponent<GameController>();
        
        Setup();
    }

    public void Setup()
    {
        spriteBounds = gc.currentMap.cameraBoundary;
        float vertExtent = gameObject.GetComponent<Camera>().orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        leftBound = spriteBounds.transform.position.x + (horzExtent - spriteBounds.sprite.bounds.size.x / 2.0f);
        rightBound = spriteBounds.transform.position.x + (spriteBounds.sprite.bounds.size.x / 2.0f - horzExtent);
        bottomBound = spriteBounds.transform.position.y + (vertExtent - spriteBounds.sprite.bounds.size.y / 2.0f);
        topBound = spriteBounds.transform.position.y + (spriteBounds.sprite.bounds.size.y / 2.0f - vertExtent);

        followPlayer = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (followPlayer)
            FollowPlayer();
    }

    void FollowPlayer ()
    {
        Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);
        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
        transform.position = pos;
    }
}